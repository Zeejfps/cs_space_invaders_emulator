import { execSync } from 'child_process';
import { cpSync, mkdirSync, readdirSync, rmSync, writeFileSync } from 'fs';
import { dirname, extname, join } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const PACKAGE_DIR = join(__dirname, '..');
const PROJECT_DIR = join(PACKAGE_DIR, '..');
const TMP  = join(PACKAGE_DIR, '.publish-tmp');
const DIST = join(PACKAGE_DIR, 'dist');
const MAIN_ASSEMBLY = 'SpaceInvadersEmulator.Wasm';

// Extensions the browser needs at runtime — everything else is a build artifact.
const KEEP = new Set(['.js', '.wasm', '.dll', '.json', '.map', '.d.ts']);

console.log('Publishing C# project…');
execSync(`dotnet publish "${PROJECT_DIR}" -c Release -o "${TMP}"`, { stdio: 'inherit' });

rmSync(DIST, { recursive: true, force: true });
mkdirSync(DIST, { recursive: true });

const copiedFiles = [];
for (const entry of readdirSync(TMP, { withFileTypes: true })) {
  if (!entry.isFile() || !KEEP.has(extname(entry.name))) continue;
  cpSync(join(TMP, entry.name), join(DIST, entry.name));
  copiedFiles.push(entry.name);
}
rmSync(TMP, { recursive: true, force: true });
console.log(`Copied ${copiedFiles.length} dotnet files.`);

// Generate dotnet.boot.js — the asset manifest dotnet.js needs on startup.
// The plain browser-wasm SDK doesn't generate this (only Blazor does), so we build it
// from the copied file list to keep it in sync with whatever dotnet publish emits.
const assets = copiedFiles.flatMap(file => {
  if (file === 'dotnet.native.wasm') return [{ name: file, behavior: 'dotnetwasm' }];
  if (extname(file) === '.dll')      return [{ name: file, behavior: 'assembly' }];
  return [];
});
writeFileSync(
  join(DIST, 'dotnet.boot.js'),
  `export default ${JSON.stringify({ mainAssemblyName: MAIN_ASSEMBLY, globalizationMode: 'invariant', assets }, null, 2)};\n`,
);
console.log(`Generated dotnet.boot.js (${assets.length} assets).`);

console.log('Compiling TypeScript…');
execSync('npx tsc', { stdio: 'inherit', cwd: PACKAGE_DIR });

writeFileSync(
  join(DIST, 'package.json'),
  JSON.stringify({ name: 'space-invaders-emulator', version: '0.1.0', type: 'module', main: 'index.js', types: 'index.d.ts' }, null, 2) + '\n',
);
console.log('Done → dist/');
