using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Lhld()
    {
        var address = FetchWord();
        Rhl = _mmu.ReadWord(address);
        return 16;
    }
}
