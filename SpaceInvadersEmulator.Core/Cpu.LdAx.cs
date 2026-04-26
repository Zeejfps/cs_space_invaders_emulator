using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu 
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LdAxB()
    {
        Ra = _mmu.Read(Rbc);
        return 7;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LdAxD()
    {
        Ra = _mmu.Read(Rde);
        return 7;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LdA()
    {
        var lo = Fetch();
        var hi = Fetch();
        var address = (hi << 8) | lo;
        Ra = _mmu.Read((ushort)address);
        return 13;
    }
}