namespace SpaceInvadersEmulator.Core;

public interface IAudio
{
    void SetUfoLoopActive(bool active);
    void PlayShot();
    void PlayPlayerDied();
    void PlayInvaderDied();
    void PlayExtraLifeGained();
    void PlayFleetMoved(int step);
    void PlayUfoHit();
}
