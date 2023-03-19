// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET')
        return;
    if (event.request.url && event.request.url.includes('/openid/'))
        return;
    event.respondWith(onFetch(event));
});
self.addEventListener('message', messageEvent => {
    if (messageEvent.data === 'skipWaiting') return self.skipWaiting();
});

const blazorKey = "blazor-resources-/"
const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const imgCacheName = 'webp-cache';
const offlineAssetsInclude = [/\.dll$/, /\.blat$/, /\.dat$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.webp/, /\.jpe?g$/, /\.gif$/, /\.ico$/];
const offlineAssetsExclude = [/^service-worker\.js$/, /^index\..*\.html$/];
const cdnBase = "https://fcdn.satroki.tech/";
const imgCdnBase = "https://pcr-1252403488.file.myqcloud.com/";
const origin = "https://pcr.satroki.tech/";

async function onInstall(event) {
    console.info('Service worker: OnInstall');

    // Fetch and cache all matching items from the assets manifest
    const assets = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)));
    let cache = await caches.open(cacheName);
    let bzCache = await caches.open(blazorKey);
    let oldCache = null;
    let cacheKeys = await caches.keys();
    let oldKeys = cacheKeys.filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
    if (oldKeys && oldKeys.length)
        oldCache = await caches.open(oldKeys[0]);
    console.info('Service worker: Check Cache');
    for (var i = 0; i < assets.length; i++) {
        let asset = assets[i];
        let cdnUrl = `${cdnBase}${asset.url}?v=${asset.hash}`;
        let bzUrl = `${origin}${asset.url}.${asset.hash}`;
        let request = new Request(cdnUrl, { integrity: asset.hash });
        let m = (oldCache && await oldCache.match(cdnUrl)) || (bzCache && await bzCache.match(bzUrl))
        if (m) {
            await cache.put(request, m);
        }
        else {
            console.info('Service worker: Update ' + asset.url);
            await cache.add(request);
        }
    }
}

async function onActivate(event) {
    console.info('Service worker: OnActivate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));
}

async function onFetch(event) {
    let cachedResponse = null;
    if (event.request.method === 'GET') {
        // For all navigation requests, try to serve index.html from cache
        // If you need some URLs to be server-rendered, edit the following check to exclude those URLs
        const shouldServeIndexHtml = event.request.mode === 'navigate';

        const request = shouldServeIndexHtml ? 'index.html' : event.request;
        if (request.url && request.url.endsWith('.webp')) {
            const icache = await caches.open(imgCacheName);
            let resp = await icache.match(request) || await icache.match(request.url.replace(origin, imgCdnBase));
            if (!resp || !resp.ok) {
                resp = await fetch(new Request(request.url, { mode: 'cors', cache: "reload" }));
                if (resp.ok) {
                    await icache.put(request, resp.clone());
                    return resp;
                }
            } else {
                return resp;
            }
        }
        else {
            const cache = await caches.open(cacheName);
            cachedResponse = await cache.match(request, { ignoreSearch: true });
        }
    }

    return cachedResponse || fetch(event.request);
}
