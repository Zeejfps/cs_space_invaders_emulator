namespace SpaceInvadersEmulator.Core;

public interface IAudio
{
    void UfoLoop(bool active);
    void PlayShot();
    void PlayPlayerDied();
    void PlayInvaderDied();
    void ExtendedPlay();
    void PlayFleetMoved(int step);
    void PlayUfoHit();
}
