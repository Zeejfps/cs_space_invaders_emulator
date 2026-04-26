using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBb()
    {
        return 5;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBc()
    {
        Rb = Rc;
        return 5;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBd()
    {
        Rb = Rd;
        return 5;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBe()
    {
        Rb = Re;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBh()
    {
        Rb = Rh;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBl()
    {
        Rb = Rl;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBa()
    {
        Rb = Ra;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBm()
    {
        Rb = _mmu.Read(Hl);
        return 7;
    }
}