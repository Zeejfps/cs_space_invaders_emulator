using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LxiB()
    {
        Rbc = FetchWord();
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LxiD()
    {
        Rde = FetchWord();
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LxiH()
    {
        Rhl = FetchWord();
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int LxiSp()
    {
        Sp = FetchWord();
        return 10;
    }
}
