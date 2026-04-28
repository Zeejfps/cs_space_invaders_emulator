using System.Runtime.InteropServices.JavaScript;

namespace SpaceInvadersEmulator.Wasm;

internal static partial class Interop
{
    [JSImport("globalThis.siAudio.setUfoLoopActive")]
    internal static partial void SetUfoLoopActive(bool active);

    [JSImport("globalThis.siAudio.playShot")]
    internal static partial void PlayShot();

    [JSImport("globalThis.siAudio.playPlayerDied")]
    internal static partial void PlayPlayerDied();

    [JSImport("globalThis.siAudio.playInvaderDied")]
    internal static partial void PlayInvaderDied();

    [JSImport("globalThis.siAudio.playExtraLifeGained")]
    internal static partial void PlayExtraLifeGained();

    [JSImport("globalThis.siAudio.playFleetMoved")]
    internal static partial void PlayFleetMoved(int step);

    [JSImport("globalThis.siAudio.playUfoHit")]
    internal static partial void PlayUfoHit();
}
