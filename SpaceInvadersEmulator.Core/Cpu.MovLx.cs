using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLb()
    {
        Rl = Rb;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLc()
    {
        Rl = Rc;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLd()
    {
        Rl = Rd;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLe()
    {
        Rl = Re;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLh()
    {
        Rl = Rh;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLl()
    {
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLa()
    {
        Rl = Ra;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLm()
    {
        Rl = _mmu.Read(Rhl);
        return 7;
    }
}
