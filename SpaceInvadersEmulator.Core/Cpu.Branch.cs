using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rnz()
    {
        if ((Flags & CpuFlags.Z) != 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rnc()
    {
        if ((Flags & CpuFlags.C) != 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rpo()
    {
        if ((Flags & CpuFlags.P) != 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rp()
    {
        if ((Flags & CpuFlags.S) != 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rz()
    {
        if ((Flags & CpuFlags.Z) == 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rcy()
    {
        if ((Flags & CpuFlags.C) == 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rpe()
    {
        if ((Flags & CpuFlags.P) == 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rm()
    {
        if ((Flags & CpuFlags.S) == 0)
            return 5;

        Pc = _mmu.ReadWord(Sp);
        Sp += 2;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Jnz()
    {
        var address = FetchWord();
        if ((Flags & CpuFlags.Z) == 0)
            Pc = address;
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Jnc()
    {
        var address = FetchWord();
        if ((Flags & CpuFlags.C) == 0)
            Pc = address;
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Jpo()
    {
        var address = FetchWord();
        if ((Flags & CpuFlags.P) == 0)
            Pc = address;
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Jp()
    {
        var address = FetchWord();
        if ((Flags & CpuFlags.S) == 0)
            Pc = address;
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Jmp()
    {
        Pc = FetchWord();
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Cnz()
    {
        var address = FetchWord();
        if ((Flags & CpuFlags.Z) != 0)
            return 11;
        Sp -= 2;
        _mmu.WriteWord(Sp, Pc);
        Pc = address;
        return 17;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Cnc()
    {
        var address = FetchWord();
        if ((Flags & CpuFlags.C) != 0)
            return 11;
        Sp -= 2;
        _mmu.WriteWord(Sp, Pc);
        Pc = address;
        return 17;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Cpo()
    {
        var address = FetchWord();
        if ((Flags & CpuFlags.P) != 0)
            return 11;
        Sp -= 2;
        _mmu.WriteWord(Sp, Pc);
        Pc = address;
        return 17;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Cp()
    {
        var address = FetchWord();
        if ((Flags & CpuFlags.S) != 0)
            return 11;
        Sp -= 2;
        _mmu.WriteWord(Sp, Pc);
        Pc = address;
        return 17;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rst(ushort vector)
    {
        Sp -= 2;
        _mmu.WriteWord(Sp, Pc);
        Pc = vector;
        return 11;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    private int Rst0() => Rst(0x0000);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    private int Rst1() => Rst(0x0008);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    private int Rst2() => Rst(0x0010);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rst3() => Rst(0x0018);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    private int Rst4() => Rst(0x0020);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    private int Rst5() => Rst(0x0028);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    private int Rst6() => Rst(0x0030);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    private int Rst7() => Rst(0x0038);
}
