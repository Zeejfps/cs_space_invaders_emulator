using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEb()
    {
        Re = Rb;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEc()
    {
        Re = Rc;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEd()
    {
        Re = Rd;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEe()
    {
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEh()
    {
        Re = Rh;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEl()
    {
        Re = Rl;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEa()
    {
        Re = Ra;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEm()
    {
        Re = _mmu.Read(Rhl);
        return 7;
    }
}
