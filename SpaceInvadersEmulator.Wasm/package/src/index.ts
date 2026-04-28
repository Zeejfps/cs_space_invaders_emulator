export interface Emulator {
  loadRom(data: Uint8Array): void;
  runFrame(): void;
  getVRam(): Uint8Array;
  writeP1Left(pressed: boolean): void;
  writeP1Right(pressed: boolean): void;
  writeP1Fire(pressed: boolean): void;
  writeCoin(pressed: boolean): void;
  writeP1Start(pressed: boolean): void;
  writeP2Start(pressed: boolean): void;
  writeP2Fire(pressed: boolean): void;
  writeP2Left(pressed: boolean): void;
  writeP2Right(pressed: boolean): void;
  setShips(count: number): void;
  setBonusLife(at1000: boolean): void;
}

export async function init(): Promise<Emulator> {
  const baseUrl = new URL('./', import.meta.url).href;
  const { dotnet } = await import('./dotnet.js');

  // Boot config is baked into dotnet.js by Microsoft.NET.Sdk.WebAssembly.
  // We only override the resource loader so URLs resolve relative to this module
  // (so consumers' bundlers can place us anywhere in their output).
  const runtime = await dotnet
    .withResourceLoader((_type: string, name: string) => new URL(name, baseUrl).href)
    .create();

  await runtime.runMain();

  const config = runtime.getConfig();
  const assemblyName: string = config.mainAssemblyName ?? 'SpaceInvadersEmulator.Wasm';
  const ex = await runtime.getAssemblyExports(assemblyName);
  const e = ex.SpaceInvadersEmulator.Wasm.Emulator;

  e.Initialize();
  const vramPtr: number = e.GetVRamPtr();
  const vramLen: number = e.GetVRamLen();

  return {
    loadRom:      (data) => e.LoadRom(data),
    runFrame:     ()     => e.RunFrame(),
    getVRam:      ()     => runtime.localHeapViewU8().subarray(vramPtr, vramPtr + vramLen),
    writeP1Left:  (p) => e.WriteP1Left(p),
    writeP1Right: (p) => e.WriteP1Right(p),
    writeP1Fire:  (p) => e.WriteP1Fire(p),
    writeCoin:    (p) => e.WriteCoin(p),
    writeP1Start: (p) => e.WriteP1Start(p),
    writeP2Start: (p) => e.WriteP2Start(p),
    writeP2Fire:  (p) => e.WriteP2Fire(p),
    writeP2Left:  (p) => e.WriteP2Left(p),
    writeP2Right: (p) => e.WriteP2Right(p),
    setShips:     (n) => e.SetShips(n),
    setBonusLife: (b) => e.SetBonusLife(b),
  };
}
