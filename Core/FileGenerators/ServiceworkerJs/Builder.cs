namespace Alga.wwwcore.Core.FileGenerators.ServiceworkerJs;

sealed class Builder
{
  public void Do(Req req)
  {
    string serviceWorkerPath = Path.Combine(req.DirectoryPath, $"serviceworker.{req.Version}.js");

    var filesToDelete = Directory.GetFiles(req.DirectoryPath, "*.js", SearchOption.TopDirectoryOnly).Where(f => Path.GetFileName(f).Contains("serviceworker"));
    foreach (var file in filesToDelete)
      File.Delete(file);

    var toCacheList = new HashSet<string>();

    if (req.Schemes != null)
      foreach (var i in req.Schemes)
      {
        if (i.Value.script != null) toCacheList.Add(i.Value.script);
        if (i.Value.style != null) toCacheList.Add(i.Value.style);
      }

    if (req?.CacheUrls != null)
      foreach (var i in req.CacheUrls)
        toCacheList.Add(i);

    if (!string.IsNullOrEmpty(req.OfflinePageUrl)) toCacheList.Add(req.OfflinePageUrl);

    var array = string.Join(",", new[] { "'" + "/" + "'" }.Concat(toCacheList.Select(i => $"'{i}'")));

    var cacheName = $"SWStaticCache-{req.Version}";

    string code = $@"
const CACHE_NAME = '{cacheName}';
const URLs_TO_CACHE = [{array}];
const OFFLINE_PAGE = '/offline';
const staticExtensions = ['.html', '.js', '.css', '.svg', '.woff2', '.woff', '.ttf', '.eot'];
const TIMESTAMP_SKIP_RE = /\.([0-9]{12})\.min\.(js|css|woff2?)$/i;
const MEDIA_SKIP_RE = /\.(?:png|jpe?g|gif|webp|svg)$/i;
const MANIFEST_SKIP_RE = /\/manifest\.[a-zA-Z0-9]+\.json$/i;

let cachePromise = null;

self.addEventListener('install', (event) => {{
  event.waitUntil(
    caches.open(CACHE_NAME) // Open the cache
      .then((cache) => {{
        cachePromise = Promise.resolve(cache);
        self.skipWaiting(); // Instant activation

        // For each URL create a chain cache.add(url).catch…
        // Safe caching with error handling for each URL

        const addOps = URLs_TO_CACHE.map((url) =>
          cache.add(url).catch(error => {{
            console.warn(`[ServiceWorker] Не удалось кэшировать ${{url}}:`, error);
            // Return success to not break the entire Promise.all
            return null;
          }})
        );

        // Use Promise.allSettled instead of Promise.all

        return Promise.allSettled(addOps).then(results => {{
          const successful = results.filter(r => r.status === 'fulfilled').length;
          const failed = results.filter(r => r.status === 'rejected').length;

          console.log(`[ServiceWorker] Caching completed: ${{successful}} successful, ${{failed}} failed`);
        }});
      }})
      .then(() => {{
        console.log('[ServiceWorker] Installation completed with partial caching.');
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

      await self.clients.claim();
      //console.log('[ServiceWorker] Активация завершена.');
    }} catch (e) {{
      console.error('[ServiceWorker] Ошибка активации:', e);
    }}
  }})());
}});

self.addEventListener('fetch', (event) => {{
    const req = event.request;

    if (req.mode === 'navigate') return;

    if (req.method !== 'GET') return;

    const url = new URL(req.url); 
    if (TIMESTAMP_SKIP_RE.test(url.pathname) || MEDIA_SKIP_RE.test(url.pathname) || MANIFEST_SKIP_RE.test(url.pathname)) {{
        event.respondWith(fetch(req));
        return;
    }}

    event.respondWith((async () => {{
        const isStatic = staticExtensions.some(ext => url.pathname.endsWith(ext));
        const cache = await cachePromise || await caches.open(CACHE_NAME);

        if (isStatic) {{
            const cached = await cache.match(req);
            if (cached) return cached;
        }}

        try {{
          const networkResponse = await fetch(req);
          const cacheControl = networkResponse.headers.get('Cache-Control');
          if (networkResponse.status === 200 && !cacheControl?.includes('no-store')) {{
              await cache.put(req, networkResponse.clone()).catch(e => {{ console.error('Ошибка сохранения в кеш:', e); }});
          }}
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
}

// sealed class Builder
// {
//   public void Do(Req req)
//   {
//     string serviceWorkerPath = Path.Combine(req.DirectoryPath, $"serviceworker.{req.Version}.js");

//     var filesToDelete = Directory.GetFiles(req.DirectoryPath, "*.js", SearchOption.TopDirectoryOnly).Where(f => Path.GetFileName(f).Contains("serviceworker"));
//     foreach (var file in filesToDelete)
//       File.Delete(file);

//     var toCacheList = new HashSet<string>();

//     if (req.Schemes != null)
//       foreach (var i in req.Schemes)
//       {
//         if (i.Value.script != null) toCacheList.Add(i.Value.script);
//         if (i.Value.style != null) toCacheList.Add(i.Value.style);
//       }

//     if (req?.CacheUrls != null)
//       foreach (var i in req.CacheUrls)
//         toCacheList.Add(i);

//     if (!string.IsNullOrEmpty(req.OfflinePageUrl)) toCacheList.Add(req.OfflinePageUrl);

//     var array = string.Join(",", new[] { "'" + "/" + "'" }.Concat(toCacheList.Select(i => $"'{i}'")));

//     var cacheName = $"SWStaticCache-{req.Version}";

//     string code = $@"
// self.addEventListener('fetch', event => {{
//   // ничего не делаем → браузер просто пропустит запрос дальше
// }});
// ";
//     File.WriteAllText(serviceWorkerPath, code);
//   }
// }


// class ServiceworkerJs
// {
//   internal void Build(Models.Config config, FrozenDictionary<string, Models.SchemeJsonM>? schemes, FrozenDictionary<string, HashSet<string>>? modules)
//   {
//     try
//     {
//       var toCacheList = new HashSet<string>();

//       if (schemes != null)
//         foreach (var i in schemes)
//         {
//           if (i.Value.script != null) toCacheList.Add(i.Value.script);
//           if (i.Value.style != null) toCacheList.Add(i.Value.style);
//           // if (i.Value.modules != null)
//           //   foreach (var j in i.Value.modules)
//           //   {
//           //     if (modules.TryGetValue(j, out var val))
//           //       foreach (var u in val)
//           //         toCacheList.Add(u);
//           //   }
//         }

//       if (config?.cacheUrls != null)
//         foreach (var i in config.cacheUrls)
//           toCacheList.Add(i);

//       if (config?.offlinePageUrl != null) toCacheList.Add(config.offlinePageUrl);

//       string rootPath = Models.RuntimeContext.RootPathFull;
//       string serviceWorkerPath = Path.Combine(rootPath, $"serviceworker.{Models.RuntimeContext.BuildVersion}.js");

//       var filesToDelete = Directory.GetFiles(rootPath, "*.js", SearchOption.TopDirectoryOnly).Where(f => Path.GetFileName(f).Contains("serviceworker"));
//       foreach (var file in filesToDelete)
//         File.Delete(file);

//       var array = string.Join(",", new[] { "'" + "/" + "'" }.Concat(toCacheList.Select(i => $"'{i}'")));

//       var cacheName = $"SWStaticCache-{Models.RuntimeContext.BuildVersion}";

//       string code = $@"
// const CACHE_NAME = '{cacheName}';
// const URLs_TO_CACHE = [{array}];
// const OFFLINE_PAGE = '/offline';
// const staticExtensions = ['.html', '.js', '.css', '.svg', '.woff2', '.woff', '.ttf', '.eot'];
// const TIMESTAMP_SKIP_RE = /\.([0-9]{12})\.min\.(js|css|woff2?)$/i;
// const MEDIA_SKIP_RE = /\.(?:png|jpe?g|gif|webp|svg)$/i;
// const MANIFEST_SKIP_RE = /\/manifest\.[a-zA-Z0-9]+\.json$/i;


// self.addEventListener('install', (event) => {{
//   event.waitUntil(
//     caches.open(CACHE_NAME) // Open the cache
//       .then((cache) => {{
//         self.skipWaiting(); // Instant activation

//         // For each URL create a chain cache.add(url).catch…
//         // Safe caching with error handling for each URL

//         const addOps = URLs_TO_CACHE.map((url) =>
//           cache.add(url).catch(error => {{
//             console.warn(`[ServiceWorker] Не удалось кэшировать ${{url}}:`, error);
//             // Return success to not break the entire Promise.all
//             return null;
//           }})
//         );

//         // Use Promise.allSettled instead of Promise.all
//         return Promise.allSettled(addOps).then(results => {{
//           const successful = results.filter(r => r.status === 'fulfilled').length;
//           const failed = results.filter(r => r.status === 'rejected').length;

//           console.log(`[ServiceWorker] Caching completed: ${{successful}} successful, ${{failed}} failed`);
//         }});
//       }})
//       .then(() => {{
//         console.log('[ServiceWorker] Installation completed with partial caching.');
//       }})
//   );
// }});

// self.addEventListener('activate', (event) => {{
//   event.waitUntil((async () => {{
//     try {{
//       const cacheNames = await caches.keys();
//       await Promise.all(
//         cacheNames
//           .filter(name => name !== CACHE_NAME)
//           .map(name  => caches.delete(name))
//       );

//       /* —‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑ */
//       // 1. Проверяем поддержку
//       if (self.registration.navigationPreload) {{
//         // 2. Включаем Navigation Preload для всех будущих navigate‑запросов
//         await self.registration.navigationPreload.enable();
//       }}
//       /* —‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑‑ */

//       await self.clients.claim();
//       //console.log('[ServiceWorker] Активация завершена.');
//     }} catch (e) {{
//       console.error('[ServiceWorker] Ошибка активации:', e);
//     }}
//   }})());
// }});

// self.addEventListener('fetch', (event) => {{
//     if (event.request.method !== 'GET') return;

//     const req = event.request;
//     const url = new URL(req.url); 
//     if (TIMESTAMP_SKIP_RE.test(url.pathname) || MEDIA_SKIP_RE.test(url.pathname) || MANIFEST_SKIP_RE.test(url.pathname)) {{
//         event.respondWith(fetch(req));
//         return;
//     }}

//     event.respondWith((async () => {{
//       /* --- 1. NAVIGATION PRELOAD ---------------------------------- */
//       if (req.mode === 'navigate' && self.registration.navigationPreload) {{
//         try {{
//           const preloadResp = await event.preloadResponse;  // может reject‑нуться
//           if (preloadResp) {{
//             return preloadResp;       // сеть уже дала HTML, offline не нужен
//           }}
//         }} catch (err) {{
//           console.warn('[ServiceWorker] Navigation‑preload error:', err);
//         }}
//       }}
//       /* ------------------------------------------------------------ */

//         const isStatic = staticExtensions.some(ext => url.pathname.endsWith(ext));
//         const cache = await caches.open(CACHE_NAME);

//         if (isStatic) {{
//             const cached = await cache.match(req);
//             if (cached) return cached;
//         }}

//         try {{
//           const networkResponse = await fetch(req);
//           const cacheControl = networkResponse.headers.get('Cache-Control');
//           if (networkResponse.status === 200 && !cacheControl?.includes('no-store')) {{
//               await cache.put(req, networkResponse.clone()).catch(e => {{ console.error('Ошибка сохранения в кеш:', e); }});
//           }}
//           return networkResponse;
//         }} catch (e) {{ 
//             console.warn('[ServiceWorker] Сетевая ошибка:', e);

//             // После того как ошибка в сети произошла, проверяем кеш
//             const cachedResponse = await cache.match(req);
//             if (cachedResponse) {{
//                 console.log('[ServiceWorker] Ответ найден в кеше:', req.url); // Логируем, если нашли в кеше
//                 return cachedResponse;
//             }}

//             const acceptHeader = req.headers.get('Accept') || '';
//             const isHtml = acceptHeader.includes('text/html');

//             if(isHtml) {{
//                 const cached = await cache.match(OFFLINE_PAGE);
//                 if (cached) return cached;

//                 // Базовая offline-страница
//                 return new Response(
//                     `<h1>Offline Mode</h1><p>The application is unavailable without internet access</p><p><a href='/' style='color: blue;'>Go to the homepage</a></p>`,
//                     {{ headers: {{ 'Content-Type': 'text/html' }} }}
//                 );
//             }}
//         }}

//         return new Response('Offline', {{ status: 503, statusText: 'Offline' }});
//     }})());
// }});
// ";

//       File.WriteAllText(serviceWorkerPath, code);
//     }
//     catch { }
//   }
// }