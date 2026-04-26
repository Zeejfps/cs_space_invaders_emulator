using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rnz()
    {
        if ((Flags & CpuFlags.Z) != 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rnc()
    {
        if ((Flags & CpuFlags.C) != 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rpo()
    {
        if ((Flags & CpuFlags.P) != 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rp()
    {
        if ((Flags & CpuFlags.S) != 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }
}
