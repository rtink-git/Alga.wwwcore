using System.Collections.Frozen;
namespace Alga.wwwcore;

class ServiceworkerJs
{
  internal void Build(Models.Config config, FrozenDictionary<string, Models.SchemeJsonM>? schemes, FrozenDictionary<string, HashSet<string>>? modules)
  {
    try
    {
      var toCacheList = new HashSet<string>();

      foreach (var i in schemes)
      {
        if (i.Value.script != null) toCacheList.Add(i.Value.script);
        if (i.Value.style != null) toCacheList.Add(i.Value.style);
        // if (i.Value.modules != null)
        //   foreach (var j in i.Value.modules)
        //   {
        //     if (modules.TryGetValue(j, out var val))
        //       foreach (var u in val)
        //         toCacheList.Add(u);
        //   }
      }

      if (config?.cacheUrls != null)
        foreach (var i in config.cacheUrls)
          toCacheList.Add(i);

      if (config?.offlinePageUrl != null) toCacheList.Add(config.offlinePageUrl);

      string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
      string serviceWorkerPath = Path.Combine(wwwrootPath, "serviceworker.js");

      var array = string.Join(",", new[] { "'" + "/" + "'" }.Concat(toCacheList.Select(i => $"'{i}'")));

      var cacheName = $"RTSWStaticCache-{config?.CurrentVersion}";

            string code = $@"
const CACHE_NAME = '{cacheName}';
const URLs_TO_CACHE = [{array}];
const OFFLINE_PAGE = '/offline';
const staticExtensions = ['.html', '.js', '.css', '.svg', '.woff2', '.woff', '.ttf', '.eot'];
const TIMESTAMP_SKIP_RE = /\.([0-9]{12})\.min\.(js|css|png|woff2?)$/i;
const MEDIA_SKIP_RE = /\.(?:png|jpe?g|gif|webp)$/i;


self.addEventListener('install', (event) => {{
  event.waitUntil(
    caches.open(CACHE_NAME)                       // ① открываем кэш
      .then((cache) => {{
        self.skipWaiting();                       // ② мгновенная активация
        // ③ для каждого URL создаём цепочку cache.add(url).catch…
        const addOps = URLs_TO_CACHE.map((url) =>
          cache.add(url)
        );
        // ④ ждём, пока завершатся все операции addOps (успех + ошибки)
        return Promise.all(addOps);
      }})
      .then(() => {{
        console.log('[ServiceWorker] Установка завершена с частичным кэшированием.');
      }})
  );
}});

self.addEventListener('activate', (event) => {{
  event.waitUntil((async () => {{
    try {{
      const cacheNames = await caches.keys();
      await Promise.all(
        cacheNames
          .filter(name => name !== CACHE_NAME)
          .map(name  => caches.delete(name))
      );

      /* —‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑ */
      // 1. Проверяем поддержку
      if (self.registration.navigationPreload) {{
        // 2. Включаем Navigation Preload для всех будущих navigate‑запросов
        await self.registration.navigationPreload.enable();
      }}
      /* —‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑ */

      await self.clients.claim();
      //console.log('[ServiceWorker] Активация завершена.');
    }} catch (e) {{
      console.error('[ServiceWorker] Ошибка активации:', e);
    }}
  }})());
}});

self.addEventListener('fetch', (event) => {{
    if (event.request.method !== 'GET') return;

    const req = event.request;
    const url = new URL(req.url); 
    if (TIMESTAMP_SKIP_RE.test(url.pathname) || MEDIA_SKIP_RE.test(url.pathname)) return;

    event.respondWith((async () => {{
      /* --- 1. NAVIGATION PRELOAD ---------------------------------- */
      if (req.mode === 'navigate' && self.registration.navigationPreload) {{
        try {{
          const preloadResp = await event.preloadResponse;  // может reject‑нуться
          if (preloadResp) {{
            return preloadResp;       // сеть уже дала HTML, offline не нужен
          }}
        }} catch (err) {{
          console.warn('[ServiceWorker] Navigation‑preload error:', err);
        }}
      }}
      /* ------------------------------------------------------------ */

        const isStatic = staticExtensions.some(ext => url.pathname.endsWith(ext));
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
    }
    catch { }
  }
}