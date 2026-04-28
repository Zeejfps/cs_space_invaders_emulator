import { execSync } from 'child_process';
import { cpSync, existsSync, mkdirSync, readdirSync, rmSync, writeFileSync } from 'fs';
import { dirname, extname, join } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const PACKAGE_DIR = join(__dirname, '..');
const PROJECT_DIR = join(PACKAGE_DIR, '..');
const TMP  = join(PACKAGE_DIR, '.publish-tmp');
const DIST = join(PACKAGE_DIR, 'dist');

console.log('Publishing C# project…');
execSync(`dotnet publish "${PROJECT_DIR}" -c Release -o "${TMP}"`, { stdio: 'inherit' });

// Microsoft.NET.Sdk.WebAssembly emits the AppBundle under wwwroot/_framework/.
// Boot manifest is embedded inside dotnet.js (no separate dotnet.boot.json), so we
// just mirror _framework/ verbatim — content-hashed filenames and all.
const FRAMEWORK = join(TMP, 'wwwroot', '_framework');
if (!existsSync(FRAMEWORK)) throw new Error(`Expected ${FRAMEWORK} after publish`);

rmSync(DIST, { recursive: true, force: true });
mkdirSync(DIST, { recursive: true });

let copied = 0;
for (const entry of readdirSync(FRAMEWORK, { withFileTypes: true })) {
  if (!entry.isFile()) continue;
  // Skip pre-compressed siblings — npm consumers' bundlers/servers handle compression.
  if (entry.name.endsWith('.br') || entry.name.endsWith('.gz')) continue;
  cpSync(join(FRAMEWORK, entry.name), join(DIST, entry.name));
  copied++;
}
console.log(`Copied ${copied} runtime files from _framework/.`);

// dotnet.d.ts isn't part of publish output — pull it from the runtime pack so
// src/dotnet.d.ts stays in sync with whatever runtime version we just published against.
const dotnetRoot = execSync('dotnet --info', { encoding: 'utf8' })
  .split('\n').find(l => l.includes('Base Path:'))?.split('Base Path:')[1]?.trim()
  ?.replace(/\/sdk\/[^/]+\/?$/, '');
if (!dotnetRoot) throw new Error('Could not determine dotnet root from `dotnet --info`');
const packRoot = join(dotnetRoot, 'packs', 'Microsoft.NETCore.App.Runtime.Mono.browser-wasm');
const versions = readdirSync(packRoot).sort();
const dts = join(packRoot, versions[versions.length - 1], 'runtimes', 'browser-wasm', 'native', 'dotnet.d.ts');
cpSync(dts, join(PACKAGE_DIR, 'src', 'dotnet.d.ts'));
cpSync(dts, join(DIST, 'dotnet.d.ts'));
console.log(`Updated dotnet.d.ts from ${versions[versions.length - 1]}.`);

rmSync(TMP, { recursive: true, force: true });

console.log('Compiling TypeScript…');
execSync('npx tsc', { stdio: 'inherit', cwd: PACKAGE_DIR });

writeFileSync(
  join(DIST, 'package.json'),
  JSON.stringify({ name: 'space-invaders-emulator', version: '0.1.0', type: 'module', main: 'index.js', types: 'index.d.ts' }, null, 2) + '\n',
);
console.log('Done → dist/');
