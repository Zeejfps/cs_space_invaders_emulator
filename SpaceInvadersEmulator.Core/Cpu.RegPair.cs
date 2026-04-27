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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Dad(ushort value)
    {
        var result = Rhl + value;
        Flags = result > 0xFFFF ? (Flags | CpuFlags.C) : (Flags & ~CpuFlags.C);
        Rhl = (ushort)result;
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DadB() => Dad(Rbc);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DadD() => Dad(Rde);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DadH() => Dad(Rhl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DadSp() => Dad(Sp);
}
