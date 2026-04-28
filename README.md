# Space Invaders Emulator

A Taito 8080 arcade emulator written in C#, with a browser frontend.

Play it: [invaders.builtbyzee.com](https://invaders.builtbyzee.com)

## Why

A learning project — built to dig into emulation past the toy stage. Followed a [Chip8 emulator](https://github.com/Zeejfps) I wrote earlier; Space Invaders was the natural next step (real Intel 8080 ISA, real timing constraints, real hardware quirks).

## Structure

- **`SpaceInvadersEmulator.Core`** — Intel 8080 CPU, MMU, ports, machine state. Pure C#, no I/O.
- **`SpaceInvadersEmulator.Core.Tests`** — CPU instruction tests.
- **`SpaceInvadersEmulator.Wasm`** — `browser-wasm` shim that exposes `Machine` to JS via `[JSExport]`. Published as an npm package.
- **`SpaceInvadersEmulator.Web`** — Svelte 5 + Vite frontend that consumes the npm package, renders the framebuffer to a canvas, and wires audio + input.

## Run locally

Requires .NET 10 SDK and Node 20+.

```
cd SpaceInvadersEmulator.Web
npm install
npm run build:wasm   # one-time, or whenever C# changes
npm run dev
```

## Tests

```
dotnet test
```

## ROMs

ROMs aren't committed. The Vite build runs a plugin (`scripts/vite-plugin-roms.ts`) that downloads each game's ROM zip from a public CDN mirror and assembles `public/roms/*.bin` on first build, cached under `.rom-cache/`. You can also drag-and-drop your own ROM files onto the page at runtime.
