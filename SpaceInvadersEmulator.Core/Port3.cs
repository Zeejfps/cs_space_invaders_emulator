namespace SpaceInvadersEmulator.Core;

[Flags]
internal enum Port3 : byte
{
    None = 0,
    UfoLoop = 1 << 0,
    Shot = 1 << 1,
    PlayerDie = 1 << 2,
    InvaderDie = 1 << 3,
    ExtendedPlay = 1 << 4,
}
