const CACHE_NAME = 'quran-daily-v2';
const CACHE_VERSION = 2;
const DATA_CACHE_NAME = 'quran-data-v1';

const STATIC_URLS = [
    '/',
    '/index.html',
    '/css/app.css',
    '/js/app.js',
    '/QuranDailyApp.styles.css',
    '/_framework/blazor.webassembly.js'
];

const DATA_URLS = [
    '/data/quran.json'
];

// Install event - cache resources
self.addEventListener('install', event => {
    event.waitUntil(
        Promise.all([
            // Cache static files
            caches.open(CACHE_NAME)
                .then(cache => cache.addAll(STATIC_URLS)),
            // Cache data files separately
            caches.open(DATA_CACHE_NAME)
                .then(cache => cache.addAll(DATA_URLS))
        ]).then(() => {
            // Force activation of new service worker
            globalThis.skipWaiting();
        })
    );
});

// Activate event - cleanup old caches
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    // Delete old cache versions
                    if (cacheName !== CACHE_NAME && cacheName !== DATA_CACHE_NAME) {
                        console.log('Deleting old cache:', cacheName);
                        return caches.delete(cacheName);
                    }
                })
            );
        }).then(() => {
            // Take control of all pages
            return globalThis.clients.claim();
        })
    );
});

// Fetch event - network first with intelligent caching
self.addEventListener('fetch', event => {
    const url = new URL(event.request.url);

    // Handle data requests with cache-then-network strategy
    if (DATA_URLS.some(dataUrl => url.pathname.includes(dataUrl))) {
        event.respondWith(
            caches.open(DATA_CACHE_NAME).then(async cache => {
                try {
                    const networkResponse = await fetch(event.request);
                    // Update cache with fresh data
                    cache.put(event.request, networkResponse.clone());
                    return networkResponse;
                } catch {
                    let cachedResponse = await cache.match(event.request);
                    // Fix for Cloudflare Pages redirected cached responses
                    if (cachedResponse?.redirected) {
                        cachedResponse = new Response(cachedResponse.body,
                            {
                                headers: cachedResponse.headers,
                                status: cachedResponse.status,
                                statusText: cachedResponse.statusText
                            });
                    }
                    return cachedResponse;
                }
            })
        );
        return;
    }

    // Handle static resources and navigation
    event.respondWith(
        fetch(event.request).catch(async () => {
            let cachedResponse = await caches.match(event.request);
            if (cachedResponse) {
                // Fix for Cloudflare Pages redirected cached responses
                if (cachedResponse.redirected) {
                    cachedResponse = new Response(cachedResponse.body,
                        {
                            headers: cachedResponse.headers,
                            status: cachedResponse.status,
                            statusText: cachedResponse.statusText
                        });
                }
                return cachedResponse;
            }
            // Fallback to index.html for SPA routes
            if (event.request.mode === 'navigate') {
                let indexResponse = await caches.match('/index.html');
                // Fix for Cloudflare Pages redirected cached responses
                if (indexResponse?.redirected) {
                    indexResponse = new Response(indexResponse.body,
                        {
                            headers: indexResponse.headers,
                            status: indexResponse.status,
                            statusText: indexResponse.statusText
                        });
                }
                return indexResponse;
            }
            return new Response('Resource not found', { status: 404 });
        })
    );
});

// Listen for cache update messages from the app
globalThis.addEventListener('message', event => {
    // Verify the origin of the message for security
    if (event.origin !== globalThis.location.origin) {
        return;
    }

    if (event.data?.type === 'SKIP_WAITING') {
        globalThis.skipWaiting();
    }
});
