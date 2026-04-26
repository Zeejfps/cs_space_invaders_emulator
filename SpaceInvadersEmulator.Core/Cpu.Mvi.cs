using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviB()
    {
        Rb = Fetch();
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviC()
    {
        Rc = Fetch();
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviD()
    {
        Rd = Fetch();
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviE()
    {
        Re = Fetch();
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviH()
    {
        Rh = Fetch();
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviL()
    {
        Rl = Fetch();
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviA()
    {
        Ra = Fetch();
        return 7;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private int MviM()
    {
        var value = Fetch();
        _mmu.Write(Hl, value);
        return 10;
    }
}
