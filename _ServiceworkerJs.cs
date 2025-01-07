using System;

namespace wwwcore;

class _ServiceworkerJs {
    internal void Create() {
        var url = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "serviceworker.js");

        if(!File.Exists(url)) {
            string serviceWorkerCode = @"
    self.addEventListener('install', event => {
        console.log('Service Worker installing...');
        event.waitUntil(
            caches.open('my-cache').then(cache => {
                console.log('Caching assets...');
                return cache.addAll([
                    '/'
                ]);
            })
        );
    });

    self.addEventListener('fetch', event => {
        console.log('Fetch event for', event.request.url);
        event.respondWith(
            caches.match(event.request).then(cachedResponse => {
                return cachedResponse || fetch(event.request);
            })
        );
    });

self.addEventListener('activate', event => {
    console.log('Service Worker activated');
});
";
            File.WriteAllText(url, serviceWorkerCode);
        }
    }
}
