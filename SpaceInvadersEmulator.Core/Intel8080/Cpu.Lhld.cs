using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core.Intel8080;

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
