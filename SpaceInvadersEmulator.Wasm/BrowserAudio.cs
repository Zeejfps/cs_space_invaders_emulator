using SpaceInvadersEmulator.Core;

namespace SpaceInvadersEmulator.Wasm;

internal sealed class BrowserAudio : IAudio
{
    public void SetUfoLoopActive(bool active) => Interop.SetUfoLoopActive(active);
    public void PlayShot() => Interop.PlayShot();
    public void PlayPlayerDied() => Interop.PlayPlayerDied();
    public void PlayInvaderDied() => Interop.PlayInvaderDied();
    public void PlayExtraLifeGained() => Interop.PlayExtraLifeGained();
    public void PlayFleetMoved(int step) => Interop.PlayFleetMoved(step);
    public void PlayUfoHit() => Interop.PlayUfoHit();
}
