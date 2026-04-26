using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int StA()
    {
        var lo = Fetch();
        var hi = Fetch();
        var address = (hi << 8) | lo;
        _mmu.Write((ushort)address, Ra);
        return 13;
    }
}