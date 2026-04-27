using SpaceInvadersEmulator.Core;

namespace SpaceInvadersEmulator.Wasm;

internal sealed class BrowserClock : IClock
{
    // Timestamps are in microseconds; performance.now() returns milliseconds.
    public long Frequency => 1_000_000;

    // Explicit add/remove avoids a nullable mismatch with the interface (event Action vs Action?).
    private event Action? _ticked;
    public event Action Ticked
    {
        add => _ticked += value;
        remove => _ticked -= value;
    }

    public long GetTimestamp() => (long)(Interop.PerformanceNow() * 1000.0);

    public void Tick() => _ticked?.Invoke();
}
