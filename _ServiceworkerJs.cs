namespace Alga.wwwcore;
class _ServiceworkerJs {
    internal void Build(Root.ConfigModel config, List<Models.SchemeJsonM> schemes, Dictionary<string, HashSet<string>> modules) {
        try {
            var toCacheList = new HashSet<string>();

            foreach(var i in schemes) {
                if(i.script != null) toCacheList.Add(i.script.Replace(".js", ".min.js"));
                if(i.style != null) toCacheList.Add(i.style.Replace(".css", ".min.css"));
                if(i.modules != null)
                    foreach(var j in i.modules) {
                        if(modules.TryGetValue(j, out var val))
                            foreach(var u in val)
                                toCacheList.Add(u.Replace(".js", ".min.js").Replace(".css", ".min.css"));
                    }
            }

            if(config?.cacheUrls != null)
                foreach(var i in config.cacheUrls)
                    toCacheList.Add(i);

            if(config?.offlinePageUrl != null) toCacheList.Add(config.offlinePageUrl);



            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string serviceWorkerPath = Path.Combine(wwwrootPath, "serviceworker.js");

            var array = string.Join(",", new[] { "'" + "/" + "'" }.Concat(toCacheList.Select(i => $"'{i}'")));

            var cacheName = $"RTSWStaticCache-{DateTime.UtcNow.ToString("yyyy-MM-dd")}-{new Random().Next()}";

string code = $@"
const CACHE_NAME = '{cacheName}';
const URLs_TO_CACHE = [{array}];
const OFFLINE_PAGE = '/offline';
const staticExtensions = ['.html', '.js', '.css', '.png', '.jpg', '.jpeg', '.svg', '.webp', '.gif', '.woff2', '.woff', '.ttf', '.eot'];

self.addEventListener('install', (event) => {{
  event.waitUntil((async () => {{
    self.skipWaiting(); // Принудительно активируем SW
    const cache = await caches.open(CACHE_NAME);
    
    for (const url of URLs_TO_CACHE) {{
      try {{
        await cache.add(url);
        console.log(`[ServiceWorker] Закеширован: ${{url}}`);
      }} catch (e) {{
        console.warn(`[ServiceWorker] Не удалось закешировать: ${{url}}`, e);
      }}
    }}

    console.log('[ServiceWorker] Установка завершена с частичным кэшированием.');
  }})());
}});

self.addEventListener('activate', (event) => {{
    event.waitUntil((async () => {{
        try {{
            const cacheNames = await caches.keys();
            await Promise.all(cacheNames.filter(name => name !== CACHE_NAME).map(name => caches.delete(name)));
            await self.clients.claim();
            console.log('[ServiceWorker] Активация завершена.');
        }} catch (e) {{
            console.error('[ServiceWorker] Ошибка активации:', e);
        }}
    }})());
}});

self.addEventListener('fetch', (event) => {{
    if (event.request.method !== 'GET') return;

    event.respondWith((async () => {{
        const req = event.request;
        const requestUrl = new URL(req.url);
        const isStatic = staticExtensions.some(ext => requestUrl.pathname.endsWith(ext));
        const cache = await caches.open(CACHE_NAME);

        if (isStatic) {{
            const cached = await cache.match(req);
            if (cached) return cached;
        }}

        try {{
            const networkResponse = await fetch(req);
            if(![403, 404, 408, 500].includes(networkResponse.status)) await cache.put(req, networkResponse.clone()).catch(e => {{ console.error('Ошибка сохранения в кеш:', e); }});
            return networkResponse;
        }} catch (e) {{ 
            console.warn('[ServiceWorker] Сетевая ошибка:', e);

            // После того как ошибка в сети произошла, проверяем кеш
            const cachedResponse = await cache.match(req);
            if (cachedResponse) {{
                console.log('[ServiceWorker] Ответ найден в кеше:', req.url); // Логируем, если нашли в кеше
                return cachedResponse;
            }}

            const acceptHeader = req.headers.get('Accept') || '';
            const isHtml = acceptHeader.includes('text/html');

            if(isHtml) {{
                const cached = await cache.match(OFFLINE_PAGE);
                if (cached) return cached;

                // Базовая offline-страница
                return new Response(
                    `<h1>Offline Mode</h1><p>The application is unavailable without internet access</p><p><a href='/' style='color: blue;'>Go to the homepage</a></p>`,
                    {{ headers: {{ 'Content-Type': 'text/html' }} }}
                );
            }}
        }}

        return new Response('Offline', {{ status: 503, statusText: 'Offline' }});
    }})());
}});
";

            File.WriteAllText(serviceWorkerPath, code);
        } catch { }
    }
}