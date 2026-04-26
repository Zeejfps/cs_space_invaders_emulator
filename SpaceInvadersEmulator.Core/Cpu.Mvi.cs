using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviB()
    {
        Rb = FetchIns();
        Pc++;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviC()
    {
        Rc = FetchIns();
        Pc++;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviD()
    {
        Rd = FetchIns();
        Pc++;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviE()
    {
        Re = FetchIns();
        Pc++;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviH()
    {
        Rh = FetchIns();
        Pc++;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviL()
    {
        Rl = FetchIns();
        Pc++;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MviA()
    {
        Ra = FetchIns();
        Pc++;
        return 7;
    }
}
