namespace SpaceInvadersEmulator.Core;

[Flags]
internal enum Port5 : byte
{
    None = 0,
    FleetMove1 = 1 << 0,
    FleetMove2 = 1 << 1,
    FleetMove3 = 1 << 2,
    FleetMove4 = 1 << 3,
    UfoHit = 1 << 4,
}
