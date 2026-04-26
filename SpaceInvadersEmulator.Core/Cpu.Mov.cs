using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBb() { return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBc() { Rb = Rc; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBd() { Rb = Rd; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBe() { Rb = Re; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBh() { Rb = Rh; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBl() { Rb = Rl; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBa() { Rb = Ra; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveBm() { Rb = _mmu.Read(Rhl); return 7; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCb() { Rc = Rb; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCc() { return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCd() { Rc = Rd; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCe() { Rc = Re; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCh() { Rc = Rh; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCl() { Rc = Rl; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCa() { Rc = Ra; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveCm() { Rc = _mmu.Read(Rhl); return 7; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDb() { Rd = Rb; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDc() { Rd = Rc; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDd() { return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDe() { Rd = Re; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDh() { Rd = Rh; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDl() { Rd = Rl; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDa() { Rd = Ra; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveDm() { Rd = _mmu.Read(Rhl); return 7; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEb() { Re = Rb; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEc() { Re = Rc; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEd() { Re = Rd; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEe() { return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEh() { Re = Rh; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEl() { Re = Rl; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEa() { Re = Ra; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveEm() { Re = _mmu.Read(Rhl); return 7; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHb() { Rh = Rb; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHc() { Rh = Rc; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHd() { Rh = Rd; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHe() { Rh = Re; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHh() { return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHl() { Rh = Rl; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHa() { Rh = Ra; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveHm() { Rh = _mmu.Read(Rhl); return 7; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLb() { Rl = Rb; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLc() { Rl = Rc; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLd() { Rl = Rd; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLe() { Rl = Re; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLh() { Rl = Rh; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLl() { return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLa() { Rl = Ra; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveLm() { Rl = _mmu.Read(Rhl); return 7; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveAb() { Ra = Rb; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveAc() { Ra = Rc; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveAd() { Ra = Rd; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveAe() { Ra = Re; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveAh() { Ra = Rh; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveAl() { Ra = Rl; return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveAa() { return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveAm() { Ra = _mmu.Read(Rhl); return 7; }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMb() { _mmu.Write(Rhl, Rb); return 7; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMc() { _mmu.Write(Rhl, Rc); return 7; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMd() { _mmu.Write(Rhl, Rd); return 7; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMe() { _mmu.Write(Rhl, Re); return 7; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMh() { _mmu.Write(Rhl, Rh); return 7; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMl() { _mmu.Write(Rhl, Rl); return 7; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int MoveMa() { _mmu.Write(Rhl, Ra); return 7; }
}
