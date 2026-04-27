using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core.Intel8080;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PopB()
    {
        Rbc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 10;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PushB()
    {
        Sp -= 2;
        _mmu.Write((ushort)(Sp + 1), Rb);
        _mmu.Write(Sp, Rc);
        return 11;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PopD()
    {
        Rde = _mmu.ReadWord(Sp);
        Sp += 2;
        return 10;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PushD()
    {
        Sp -= 2;
        _mmu.Write((ushort)(Sp + 1), Rd);
        _mmu.Write(Sp, Re);
        return 11;
    }

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PopH()
    {
        Rhl = _mmu.ReadWord(Sp);
        Sp += 2;
        return 10;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PushH()
    {
        Sp -= 2;
        _mmu.Write((ushort)(Sp + 1), Rh);
        _mmu.Write(Sp, Rl);
        return 11;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PopPsw()
    {
        Flags = (CpuFlags)_mmu.Read(Sp);
        Ra = _mmu.Read((ushort)(Sp + 1));
        Sp += 2;
        return 10;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PushPsw()
    {
        Sp -= 2;
        _mmu.Write((ushort)(Sp + 1), Ra);
        _mmu.Write(Sp, (byte)Flags);
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Xthl()
    {
        var temp = Rhl;
        Rhl = _mmu.ReadWord(Sp);
        _mmu.WriteWord(Sp, temp);
        return 18;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Xchg()
    {
        (Rh, Rd) = (Rd, Rh);
        (Rl, Re) = (Re, Rl);
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Sphl()
    {
        Sp = Rhl;
        return 5;
    }
}