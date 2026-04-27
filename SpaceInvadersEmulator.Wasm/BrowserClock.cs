using SpaceInvadersEmulator.Core;

namespace SpaceInvadersEmulator.Wasm;

internal sealed class BrowserClock : IClock
{
    // Timestamps are in microseconds; performance.now() returns milliseconds.
    public long Frequency => 1_000_000;
    
    public event Action? Ticked;

    public long GetTimestamp() => (long)(Interop.PerformanceNow() * 1000.0);

    public void Tick() => Ticked?.Invoke();
}
