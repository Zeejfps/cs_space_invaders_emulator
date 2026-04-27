using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Di()
    {
        InterruptEnabled = false;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Ei()
    {
        _enableInterruptsOnNextInstruction = true;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int In()
    {
        var port = Fetch();
        Ra = _io.ReadPort(port);
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Out()
    {
        var port = Fetch();
        _io.WritePort(port, Ra);
        return 10;
    }
}
