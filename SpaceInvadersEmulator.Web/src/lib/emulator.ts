// Typed wrappers around the [JSExport] methods from SpaceInvadersEmulator.Wasm.
// Runtime files are served from /wasm/ by the serveWasmAssets() Vite plugin.
import { init, type Emulator } from 'space-invaders-emulator';

let _emu: Emulator | null = null;
let _initPromise: Promise<void> | null = null;

export function initEmulator(): Promise<void> {
  if (!_initPromise) {
    _initPromise = init({ baseUrl: '/wasm/' }).then((emu) => {
      _emu = emu;
    });
  }
  return _initPromise;
}

export function isEmulatorReady(): boolean { return _emu !== null; }

export function loadRom(data: Uint8Array): void    { _emu!.loadRom(data); }
export function runFrame(): void                    { _emu!.runFrame(); }
export function getVRam(): Uint8Array               { return _emu!.getVRam(); }
export function writeP1Left(p: boolean): void       { _emu!.writeP1Left(p); }
export function writeP1Right(p: boolean): void      { _emu!.writeP1Right(p); }
export function writeP1Fire(p: boolean): void       { _emu!.writeP1Fire(p); }
export function writeCoin(p: boolean): void         { _emu!.writeCoin(p); }
export function writeP1Start(p: boolean): void      { _emu!.writeP1Start(p); }
export function writeP2Start(p: boolean): void      { _emu!.writeP2Start(p); }
export function writeP2Fire(p: boolean): void       { _emu!.writeP2Fire(p); }
export function writeP2Left(p: boolean): void       { _emu!.writeP2Left(p); }
export function writeP2Right(p: boolean): void      { _emu!.writeP2Right(p); }
export function setShips(n: number): void           { _emu!.setShips(n); }
export function setBonusLife(b: boolean): void      { _emu!.setBonusLife(b); }
