import type { Plugin } from 'vite';
import { unzipSync } from 'fflate';
import fs from 'node:fs/promises';
import path from 'node:path';
import { GAMES, type GameDef } from '../src/lib/games';

const CDN = (zip: string): string =>
  `https://cdn.jsdelivr.net/gh/tommojphillips/Space-Invaders@master/roms/${zip}.zip`;

const MAX_ROM_SIZE = 0x10000; // full 8080 address space

interface Logger {
  info: (m: string) => void;
  warn: (m: string) => void;
}

async function ensureRom(g: GameDef, cacheDir: string, outDir: string, log: Logger): Promise<void> {
  const outPath = path.join(outDir, g.romFile);
  try { await fs.access(outPath); return; } catch { /* not present, build it */ }

  const zipPath = path.join(cacheDir, `${g.upstreamZip}.zip`);
  let zipBytes: Uint8Array;
  try {
    zipBytes = await fs.readFile(zipPath);
  } catch {
    log.info(`fetching ${g.upstreamZip}.zip`);
    const resp = await fetch(CDN(g.upstreamZip));
    if (!resp.ok) throw new Error(`fetch ${g.upstreamZip}.zip → ${resp.status}`);
    zipBytes = new Uint8Array(await resp.arrayBuffer());
    await fs.mkdir(cacheDir, { recursive: true });
    await fs.writeFile(zipPath, zipBytes);
  }

  const entries = unzipSync(zipBytes);
  const lookup = new Map<string, Uint8Array>(
    Object.entries(entries).map(([k, v]) => [k.toLowerCase(), v]),
  );

  if (g.unverified) {
    const sizes = Array.from(lookup.entries())
      .map(([n, d]) => `${n}=${d.length}`)
      .join(', ');
    log.info(`[${g.id}] zip contents: ${sizes}`);
  }

  let total = 0;
  for (const b of g.romMap) {
    const data = lookup.get(b.name.toLowerCase());
    if (data) total = Math.max(total, b.offset + data.length);
  }
  if (total === 0) throw new Error(`[${g.id}] no banks resolved from zip`);
  if (total > MAX_ROM_SIZE) throw new Error(`[${g.id}] rom would be ${total} bytes (>64KB)`);

  const merged = new Uint8Array(total); // gaps stay 0x00
  for (const b of g.romMap) {
    const data = lookup.get(b.name.toLowerCase());
    if (!data) throw new Error(`[${g.id}] missing bank '${b.name}' in ${g.upstreamZip}.zip`);
    merged.set(data, b.offset);
  }

  await fs.mkdir(outDir, { recursive: true });
  await fs.writeFile(outPath, merged);
  log.info(`built ${g.romFile} (${merged.length} bytes)`);
}

export function romsPlugin(opts: { rootDir: string }): Plugin {
  const cacheDir = path.join(opts.rootDir, '.rom-cache');
  const outDir = path.join(opts.rootDir, 'public', 'roms');

  return {
    name: 'space-invaders-roms',
    async buildStart() {
      const log: Logger = {
        info: (m) => this.info(`[roms] ${m}`),
        warn: (m) => this.warn(`[roms] ${m}`),
      };
      // Sequential to keep CDN polite. ROM zips are <10KB each — total cost is negligible.
      for (const g of GAMES) {
        try {
          await ensureRom(g, cacheDir, outDir, log);
        } catch (e) {
          log.warn(`${g.id}: ${(e as Error).message}`);
        }
      }
    },
  };
}
