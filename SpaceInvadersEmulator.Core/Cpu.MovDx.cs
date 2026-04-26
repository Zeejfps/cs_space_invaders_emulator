using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDb()
    {
        Rd = Rb;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDc()
    {
        Rd = Rc;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDd()
    {
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDe()
    {
        Rd = Re;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDh()
    {
        Rd = Rh;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDl()
    {
        Rd = Rl;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDa()
    {
        Rd = Ra;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDm()
    {
        Rd = _mmu.Read(Rhl);
        return 7;
    }
}
