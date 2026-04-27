namespace SpaceInvadersEmulator.Core;

[Flags]
internal enum Port2 : byte
{
    None = 0,
    InsertCoin = 1 << 0,
    Player2Start = 1 << 1,
    Player1Start = 1 << 2,
    Player1Fire = 1 << 4,
    Player1Left = 1 << 5,
    Player1Right = 1 << 6,
}