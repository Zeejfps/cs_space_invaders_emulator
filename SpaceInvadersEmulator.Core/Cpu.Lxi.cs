using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LxiB()
    {
        var lo = Fetch();
        var hi = Fetch();
        Rbc = (ushort)((hi << 8) | lo);
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LxiD()
    {
        var lo = Fetch();
        var hi = Fetch();
        Rde = (ushort)((hi << 8) | lo);
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LxiH()
    {
        var lo = Fetch();
        var hi = Fetch();
        Rhl = (ushort)((hi << 8) | lo);
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LxiSp()
    {
        var lo = Fetch();
        var hi = Fetch();
        Sp = (ushort)((hi << 8) | lo);
        return 10;
    }
}
