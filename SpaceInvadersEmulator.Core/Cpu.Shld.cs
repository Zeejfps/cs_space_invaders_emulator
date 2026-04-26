using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Shld()
    {
        var address = FetchWord();
        _mmu.WriteWord(address, Rhl);
        return 16;
    }
}
