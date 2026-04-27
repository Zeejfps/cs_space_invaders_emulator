namespace SpaceInvadersEmulator.Core.Intel8080;

[Flags]
public enum CpuFlags : byte
{
    None = 0,
    C = 1,
    P = 1 << 2,
    A = 1 << 4,
    Z = 1 << 6,
    S = 1 << 7,
    All = S | Z | A | P | C,
}