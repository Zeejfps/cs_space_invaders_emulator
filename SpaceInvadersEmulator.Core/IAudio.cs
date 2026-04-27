namespace SpaceInvadersEmulator.Core;

public interface IAudio
{
    void UfoLoop(bool active);
    void Shot();
    void PlayerDie();
    void InvaderDie();
    void ExtendedPlay();
    void FleetMove(int step);
    void UfoHit();
}
