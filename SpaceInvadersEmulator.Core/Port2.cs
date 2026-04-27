namespace SpaceInvadersEmulator.Core;

[Flags]
internal enum Port2 : byte
{
    None = 0,
    ShipsBit0 = 1 << 0,
    ShipsBit1 = 1 << 1,
    BonusAt1000 = 1 << 3,
    Player2Fire = 1 << 4,
    Player2Left = 1 << 5,
    Player2Right = 1 << 6,
}
