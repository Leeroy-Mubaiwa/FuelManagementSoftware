const CACHE_NAME = 'petrochain-v1';
const OFFLINE_URL = '/offline.html';

const PRECACHE_URLS = [
    '/',
    '/css/petrochain.css',
    '/offline.html',
    '/manifest.json'
];

// Install: cache core assets
self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(CACHE_NAME).then(cache => {
            return cache.addAll(PRECACHE_URLS).catch(() => {
                // Continue even if some resources fail to cache
            });
        })
    );
    self.skipWaiting();
});

// Activate: clean old caches
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys =>
            Promise.all(
                keys.filter(k => k !== CACHE_NAME).map(k => caches.delete(k))
            )
        )
    );
    self.clients.claim();
});

// Fetch: network-first for API calls, cache-first for static assets
self.addEventListener('fetch', event => {
    const { request } = event;

    // Skip non-GET, SignalR, and non-http/https requests (like chrome-extension)
    if (request.method !== 'GET' || request.url.includes('/hubs/') || !request.url.startsWith('http')) {
        return;
    }

    // API and page requests: network-first
    if (request.mode === 'navigate' || request.url.includes('/api/')) {
        event.respondWith(
            fetch(request)
                .then(response => {
                    // Cache successful page loads
                    if (response.ok && request.mode === 'navigate') {
                        const clone = response.clone();
                        caches.open(CACHE_NAME).then(cache => cache.put(request, clone));
                    }
                    return response;
                })
                .catch(() => {
                    return caches.match(request).then(cached => {
                        return cached || caches.match(OFFLINE_URL);
                    });
                })
        );
        return;
    }

    // Static assets: cache-first
    event.respondWith(
        caches.match(request).then(cached => {
            if (cached) return cached;
            return fetch(request).then(response => {
                if (response.ok) {
                    const clone = response.clone();
                    caches.open(CACHE_NAME).then(cache => cache.put(request, clone));
                }
                return response;
            });
        })
    );
});

