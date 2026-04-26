using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PopB()
    {
        Rc = _mmu.Read(Sp);
        Rb = _mmu.Read((ushort)(Sp + 1));
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
        Re = _mmu.Read(Sp);
        Rd = _mmu.Read((ushort)(Sp + 1));
        Sp += 2;
        return 10;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int PopH()
    {
        Rl = _mmu.Read(Sp);
        Rh = _mmu.Read((ushort)(Sp + 1));
        Sp += 2;
        return 10;
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
}