using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core.Intel8080;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static CpuFlags ComputeAnaFlags(byte a, byte b, byte result)
    {
        var flags = CpuFlags.None;
        if (result == 0) flags |= CpuFlags.Z;
        if ((result & 0x80) != 0) flags |= CpuFlags.S;
        if (Parity(result)) flags |= CpuFlags.P;
        if (((a | b) & 0x08) != 0) flags |= CpuFlags.A;
        return flags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static CpuFlags ComputeLogicalFlags(byte result)
    {
        var flags = CpuFlags.None;
        if (result == 0) flags |= CpuFlags.Z;
        if ((result & 0x80) != 0) flags |= CpuFlags.S;
        if (Parity(result)) flags |= CpuFlags.P;
        return flags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Ana(byte value)
    {
        var result = (byte)(Ra & value);
        Flags = ComputeAnaFlags(Ra, value, result);
        Ra = result;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Xra(byte value)
    {
        var result = (byte)(Ra ^ value);
        Flags = ComputeLogicalFlags(result);
        Ra = result;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AnaB() => Ana(Rb);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AnaC() => Ana(Rc);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AnaD() => Ana(Rd);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AnaE() => Ana(Re);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AnaH() => Ana(Rh);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AnaL() => Ana(Rl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AnaA() => Ana(Ra);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AnaM()
    {
        var value = _mmu.Read(Rhl);
        var result = (byte)(Ra & value);
        Flags = ComputeAnaFlags(Ra, value, result);
        Ra = result;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int XraB() => Xra(Rb);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int XraC() => Xra(Rc);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int XraD() => Xra(Rd);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int XraE() => Xra(Re);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int XraH() => Xra(Rh);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int XraL() => Xra(Rl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int XraA() => Xra(Ra);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int XraM()
    {
        var value = _mmu.Read(Rhl);
        var result = (byte)(Ra ^ value);
        Flags = ComputeLogicalFlags(result);
        Ra = result;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Ora(byte value)
    {
        var result = (byte)(Ra | value);
        Flags = ComputeLogicalFlags(result);
        Ra = result;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int OraB() => Ora(Rb);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int OraC() => Ora(Rc);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int OraD() => Ora(Rd);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int OraE() => Ora(Re);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int OraH() => Ora(Rh);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int OraL() => Ora(Rl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int OraA() => Ora(Ra);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int OraM()
    {
        var value = _mmu.Read(Rhl);
        var result = (byte)(Ra | value);
        Flags = ComputeLogicalFlags(result);
        Ra = result;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Cmp(byte value)
    {
        var result = Ra - value;
        Flags = ComputeSubFlags(Ra, value, result);
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CmpB() => Cmp(Rb);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CmpC() => Cmp(Rc);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CmpD() => Cmp(Rd);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CmpE() => Cmp(Re);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CmpH() => Cmp(Rh);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CmpL() => Cmp(Rl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CmpA() => Cmp(Ra);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CmpM()
    {
        var value = _mmu.Read(Rhl);
        var result = Ra - value;
        Flags = ComputeSubFlags(Ra, value, result);
        return 7;
    }
}
