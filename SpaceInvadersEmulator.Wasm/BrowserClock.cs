using System.Diagnostics;
using SpaceInvadersEmulator.Core;

namespace SpaceInvadersEmulator.Wasm;

// Follows the same pattern as Chip8's ManualClock:
// the caller (Emulator.RunFrame) measures real elapsed time via Stopwatch and
// drives the clock by calling Advance(delta). This avoids any JSImport for
// timing and keeps all timestamp work inside managed C#.
internal sealed class BrowserClock : IClock
{
    private long _logicalTimestamp;

    public long Frequency => Stopwatch.Frequency;

    public event Action? Ticked;

    public long GetTimestamp() => _logicalTimestamp;

    public void Advance(long delta)
    {
        _logicalTimestamp += delta;
        Ticked?.Invoke();
    }
}
