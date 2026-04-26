using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCb()
    {
        Rc = Rb;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCc()
    {
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCd()
    {
        Rc = Rd;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCe()
    {
        Rc = Re;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCh()
    {
        Rc = Rh;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCl()
    {
        Rc = Rl;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCa()
    {
        Rc = Ra;
        return 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCm()
    {
        Rc = _mmu.Read(Hl);
        return 7;
    }
}
