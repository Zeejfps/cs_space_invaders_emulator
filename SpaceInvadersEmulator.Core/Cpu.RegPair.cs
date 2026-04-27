using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InxB() { Rbc = (ushort)(Rbc + 1); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InxD() { Rde = (ushort)(Rde + 1); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InxH() { Rhl = (ushort)(Rhl + 1); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InxSp() { Sp = (ushort)(Sp + 1); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcxB() { Rbc = (ushort)(Rbc - 1); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcxD() { Rde = (ushort)(Rde - 1); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcxH() { Rhl = (ushort)(Rhl - 1); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcxSp() { Sp = (ushort)(Sp - 1); return 5; }
}
