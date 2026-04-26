using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMb()
    {
        _mmu.Write(Rhl, Rb);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMc()
    {
        _mmu.Write(Rhl, Rc);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMd()
    {
        _mmu.Write(Rhl, Rd);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMe()
    {
        _mmu.Write(Rhl, Re);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMh()
    {
        _mmu.Write(Rhl, Rh);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMl()
    {
        _mmu.Write(Rhl, Rl);
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMa()
    {
        _mmu.Write(Rhl, Ra);
        return 7;
    }
}