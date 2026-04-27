namespace SpaceInvadersEmulator.Core;

[Flags]
internal enum Port2 : byte
{
    None = 0,
    Player2Fire = 1 << 4,
    Player2Left = 1 << 5,
    Player2Right = 1 << 6,
}
