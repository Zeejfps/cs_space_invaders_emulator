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
}