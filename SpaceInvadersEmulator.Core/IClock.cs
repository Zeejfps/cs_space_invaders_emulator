namespace SpaceInvadersEmulator.Core;

public interface IClock
{
    event Action Ticked;
    long Frequency { get; }
    long GetTimestamp();
}