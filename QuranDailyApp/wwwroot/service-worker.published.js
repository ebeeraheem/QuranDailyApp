//// Caution! Be sure you understand the caveats before publishing an application with
//// offline support. See https://aka.ms/blazor-offline-considerations

//self.importScripts('./service-worker-assets.js');
//self.addEventListener('install', event => event.waitUntil(onInstall(event)));
//self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
//self.addEventListener('fetch', event => event.respondWith(onFetch(event)));

//const cacheNamePrefix = 'offline-cache-';
//const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
//const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/ ];
//const offlineAssetsExclude = [ /^service-worker\.js$/ ];

//// Replace with your base path if you are hosting on a subfolder. Ensure there is a trailing '/'.
//const base = "/";
//const baseUrl = new URL(base, self.origin);
//const manifestUrlList = self.assetsManifest.assets.map(asset => new URL(asset.url, baseUrl).href);

//async function onInstall(event) {
//    console.info('Service worker: Install');

//    // Fetch and cache all matching items from the assets manifest
//    const assetsRequests = self.assetsManifest.assets
//        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
//        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
//        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
//    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));
//}

//async function onActivate(event) {
//    console.info('Service worker: Activate');

//    // Delete unused caches
//    const cacheKeys = await caches.keys();
//    await Promise.all(cacheKeys
//        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
//        .map(key => caches.delete(key)));
//}

//async function onFetch(event) {
//    let cachedResponse = null;
//    if (event.request.method === 'GET') {
//        // For all navigation requests, try to serve index.html from cache,
//        // unless that request is for an offline resource.
//        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
//        const shouldServeIndexHtml = event.request.mode === 'navigate'
//            && !manifestUrlList.some(url => url === event.request.url);

//        const request = shouldServeIndexHtml ? 'index.html' : event.request;
//        const cache = await caches.open(cacheName);
//        cachedResponse = await cache.match(request);
//    }

//    return cachedResponse || fetch(event.request);
//}

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


