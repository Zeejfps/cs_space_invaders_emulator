// Entry point required by Microsoft.NET.Sdk.WebAssembly (OutputType=Exe).
// Consumers initialize the runtime themselves via dotnet.create() and then
// call [JSExport]ed methods on Emulator — Main is never executed in that flow.
return 0;
