// Stub entry module required by Microsoft.NET.Sdk.WebAssembly (WasmMainJSPath).
// Not executed by the npm package — consumers import dotnet.js directly via package/src/index.ts
// and call dotnet.create() with the SDK-generated dotnet.boot.js manifest.
import { dotnet } from './_framework/dotnet.js';
await dotnet.create();
