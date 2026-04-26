using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHb()
    {
        Rh = Rb;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHc()
    {
        Rh = Rc;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHd()
    {
        Rh = Rd;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHe()
    {
        Rh = Re;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHh()
    {
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHl()
    {
        Rh = Rl;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHa()
    {
        Rh = Ra;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHm()
    {
        Rh = _mmu.Read(Rhl);
        return 7;
    }
}
