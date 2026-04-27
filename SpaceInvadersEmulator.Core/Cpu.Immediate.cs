using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Adi()
    {
        var imm = Fetch();
        var result = Ra + imm;
        Flags = ComputeAddFlags(Ra, imm, result);
        Ra = (byte)result;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Sui()
    {
        var imm = Fetch();
        var result = Ra - imm;
        Flags = ComputeSubFlags(Ra, imm, result);
        Ra = (byte)result;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Ani()
    {
        var imm = Fetch();
        var result = (byte)(Ra & imm);
        Flags = ComputeAnaFlags(Ra, imm, result);
        Ra = result;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Ori()
    {
        var imm = Fetch();
        var result = (byte)(Ra | imm);
        Flags = ComputeLogicalFlags(result);
        Ra = result;
        return 7;
    }
}
