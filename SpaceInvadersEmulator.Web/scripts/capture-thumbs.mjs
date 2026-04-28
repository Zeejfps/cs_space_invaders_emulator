// One-off: boot each game in headless Chromium, wait for the attract screen
// to settle, and capture the canvas to public/thumbs/<id>.png at native 224x256.
//
// Usage: dev server must be running on http://localhost:5173/
//   node scripts/capture-thumbs.mjs
//
// For each game we also do a basic sanity check: after boot, count non-zero
// pixels in the canvas. If it's near-zero or pure-noise, the ROM offset is
// almost certainly wrong — we log a warning so the user knows to check.

import { chromium } from 'playwright';
import fs from 'node:fs/promises';
import path from 'node:path';

const GAMES = ['invaders', 'invadpt2', 'ballbomb', 'lrescue'];
const BASE = 'http://localhost:5173';
const OUT_DIR = path.resolve('public/thumbs');
const ATTRACT_WAIT_MS = 15000;

await fs.mkdir(OUT_DIR, { recursive: true });

const browser = await chromium.launch();
const page = await browser.newPage({ viewport: { width: 800, height: 800 } });

// Surface page-side errors so a busted ROM doesn't silently produce a black image.
page.on('pageerror', (err) => console.error('  pageerror:', err.message));
page.on('console', (msg) => {
  if (msg.type() === 'error') console.error('  console.error:', msg.text());
});

for (const id of GAMES) {
  process.stdout.write(`[${id}] booting... `);
  await page.goto(`${BASE}/?game=${id}`, { waitUntil: 'load' });

  // Wait for canvas (rendered after WASM init + ROM fetch + first frame).
  try {
    await page.waitForSelector('canvas', { timeout: 30000 });
  } catch {
    console.error('FAILED: canvas never appeared');
    continue;
  }

  // Let the attract screen play out.
  await page.waitForTimeout(ATTRACT_WAIT_MS);

  // Capture the canvas pixels directly (224x256 at native res).
  const result = await page.evaluate(async () => {
    const c = document.querySelector('canvas');
    if (!c) return { ok: false, reason: 'no canvas' };
    // Count non-zero pixels for sanity.
    const ctx = c.getContext('2d');
    if (!ctx) return { ok: false, reason: 'no 2d context' };
    const img = ctx.getImageData(0, 0, c.width, c.height);
    let lit = 0;
    for (let i = 0; i < img.data.length; i += 4) {
      if (img.data[i] || img.data[i + 1] || img.data[i + 2]) lit++;
    }
    const total = c.width * c.height;
    const blob = await new Promise((res) => c.toBlob(res, 'image/png'));
    if (!blob) return { ok: false, reason: 'toBlob failed' };
    const buf = new Uint8Array(await blob.arrayBuffer());
    return { ok: true, w: c.width, h: c.height, lit, total, pngB64: btoa(String.fromCharCode(...buf)) };
  });

  if (!result.ok) {
    console.error(`FAILED: ${result.reason}`);
    continue;
  }

  const pct = ((result.lit / result.total) * 100).toFixed(1);
  const pngBytes = Uint8Array.from(atob(result.pngB64), (c) => c.charCodeAt(0));
  const outPath = path.join(OUT_DIR, `${id}.png`);
  await fs.writeFile(outPath, pngBytes);

  // Heuristic: a working title/attract screen typically lights 5-25% of pixels.
  // <0.5% is almost certainly a black/broken render; >50% is almost certainly garbage.
  const flag =
    result.lit < result.total * 0.005 ? ' ⚠ canvas mostly empty — ROM likely broken' :
    result.lit > result.total * 0.5   ? ' ⚠ canvas mostly lit — ROM likely garbage' : '';
  console.log(`captured ${result.w}x${result.h}, ${pct}% pixels lit → ${outPath}${flag}`);
}

await browser.close();
