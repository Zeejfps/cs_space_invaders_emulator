using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMb()
    {
        _mmu.Write(Hl, Rb);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMc()
    {
        _mmu.Write(Hl, Rc);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMd()
    {
        _mmu.Write(Hl, Rd);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMe()
    {
        _mmu.Write(Hl, Re);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMh()
    {
        _mmu.Write(Hl, Rh);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMl()
    {
        _mmu.Write(Hl, Rl);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMa()
    {
        _mmu.Write(Hl, Ra);
        return 7;
    }
}