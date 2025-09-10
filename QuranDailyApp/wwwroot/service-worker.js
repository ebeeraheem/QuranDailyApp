const CACHE_NAME = 'quran-daily-v1';
const urlsToCache = [
    '/',
    '/index.html',
    '/data/quran.json',
    '/css/app.css',
    '/QuranDailyApp.styles.css',
    '/_framework/blazor.webassembly.js'
];

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => cache.addAll(urlsToCache))
    );
});

self.addEventListener('fetch', event => {
    event.respondWith(
        caches.match(event.request)
            .then(response => response || fetch(event.request))
    );
});