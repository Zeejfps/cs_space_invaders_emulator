import { defineConfig, type Plugin } from 'vite';
import { svelte } from '@sveltejs/vite-plugin-svelte';
import tailwindcss from '@tailwindcss/vite';
import { VitePWA } from 'vite-plugin-pwa';
import path from 'node:path';
import fs from 'node:fs';
import { fileURLToPath } from 'node:url';
import { romsPlugin } from './scripts/vite-plugin-roms';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const WASM_PREFIX = '/wasm/';
const WASM_DIR = path.resolve(__dirname, '../SpaceInvadersEmulator.Wasm/package/dist');
const SKIP_FILES = new Set(['index.js', 'index.d.ts', 'assets.js', 'assets.d.ts', 'dotnet.d.ts', 'package.json']);

const MIME: Record<string, string> = {
  '.js':   'application/javascript',
  '.mjs':  'application/javascript',
  '.wasm': 'application/wasm',
  '.json': 'application/json',
  '.map':  'application/json',
};

// Make the space-invaders-emulator npm package's runtime files available at /wasm/*.
// Dev: a middleware streams files from the package's dist folder.
// Build: emit each file as a Vite asset under wasm/ in the build output.
// Avoids /@fs/ paths, fs.allow, public/ copy, etc.
function serveWasmAssets(): Plugin {
  return {
    name: 'serve-wasm-assets',
    configureServer(server) {
      if (!fs.existsSync(WASM_DIR)) {
        server.config.logger.warn(
          `[serve-wasm-assets] ${WASM_DIR} does not exist. Run \`npm run build:wasm\` first.`,
        );
      }
      server.middlewares.use((req, res, next) => {
        const url = req.url;
        if (!url || !url.startsWith(WASM_PREFIX)) return next();
        const rel = url.slice(WASM_PREFIX.length).split('?')[0];
        const filePath = path.join(WASM_DIR, rel);
        if (!filePath.startsWith(WASM_DIR)) return next();
        fs.stat(filePath, (err, stat) => {
          if (err || !stat.isFile()) {
            res.statusCode = 404;
            res.end();
            return;
          }
          res.setHeader('Content-Type', MIME[path.extname(filePath).toLowerCase()] ?? 'application/octet-stream');
          res.setHeader('Content-Length', stat.size);
          fs.createReadStream(filePath).pipe(res);
        });
      });
    },
    generateBundle() {
      if (!fs.existsSync(WASM_DIR)) {
        this.warn(`${WASM_DIR} does not exist. Run \`npm run build:wasm\` before \`vite build\`.`);
        return;
      }
      for (const name of fs.readdirSync(WASM_DIR)) {
        if (SKIP_FILES.has(name)) continue;
        const full = path.join(WASM_DIR, name);
        if (!fs.statSync(full).isFile()) continue;
        this.emitFile({
          type: 'asset',
          fileName: `wasm/${name}`,
          source: fs.readFileSync(full),
        });
      }
    },
  };
}

export default defineConfig({
  plugins: [
    tailwindcss(),
    svelte(),
    serveWasmAssets(),
    romsPlugin({ rootDir: __dirname }),
    VitePWA({
      registerType: 'autoUpdate',
      includeAssets: ['icons/*.svg', 'thumbs/*.png', 'sounds/*.wav'],
      manifest: {
        name: 'Space Invaders Emulator',
        short_name: 'Invaders',
        description: 'Classic Taito 8080 arcade ports running on a C# WASM emulator.',
        theme_color: '#000000',
        background_color: '#000000',
        display: 'standalone',
        orientation: 'any',
        start_url: '/',
        icons: [
          { src: '/icons/icon.svg', sizes: 'any', type: 'image/svg+xml', purpose: 'any maskable' },
        ],
      },
      workbox: {
        globPatterns: [
          '**/*.{js,css,html,svg,png,ico,webmanifest,wav}',
          'roms/**/*.bin',
        ],
        // WASM payloads (the .NET runtime + assemblies) are several MB; eager
        // precache is rude on cellular. Cache lazily on first request instead.
        runtimeCaching: [
          {
            urlPattern: ({ url }) => url.pathname.startsWith('/wasm/'),
            handler: 'CacheFirst',
            options: {
              cacheName: 'wasm-runtime',
              expiration: { maxEntries: 30, maxAgeSeconds: 60 * 60 * 24 * 30 },
            },
          },
        ],
        maximumFileSizeToCacheInBytes: 10 * 1024 * 1024,
      },
    }),
  ],
});
