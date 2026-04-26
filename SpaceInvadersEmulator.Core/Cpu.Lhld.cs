using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Lhld()
    {
        var address = FetchWord();
        Rl = _mmu.Read(address);
        Rh = _mmu.Read((ushort)(address + 1));
        return 16;
    }
}
