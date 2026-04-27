using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Parity(byte value)
    {
        value ^= (byte)(value >> 4);
        value ^= (byte)(value >> 2);
        value ^= (byte)(value >> 1);
        return (value & 1) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static CpuFlags ComputeAddFlags(byte a, byte b, int result, int carry = 0)
    {
        var flags = CpuFlags.None;
        var byteResult = (byte)result;
        if (byteResult == 0) flags |= CpuFlags.Z;
        if ((byteResult & 0x80) != 0) flags |= CpuFlags.S;
        if (Parity(byteResult)) flags |= CpuFlags.P;
        if (result > 0xFF) flags |= CpuFlags.C;
        if ((a & 0xF) + (b & 0xF) + carry > 0xF) flags |= CpuFlags.A;
        return flags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Add(byte value)
    {
        var result = Ra + value;
        Flags = ComputeAddFlags(Ra, value, result);
        Ra = (byte)result;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Adc(byte value)
    {
        var carry = (Flags & CpuFlags.C) != 0 ? 1 : 0;
        var result = Ra + value + carry;
        Flags = ComputeAddFlags(Ra, value, result, carry);
        Ra = (byte)result;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AddB() => Add(Rb);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AddC() => Add(Rc);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AddD() => Add(Rd);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AddE() => Add(Re);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AddH() => Add(Rh);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AddL() => Add(Rl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AddA() => Add(Ra);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AddM()
    {
        var value = _mmu.Read(Rhl);
        var result = Ra + value;
        Flags = ComputeAddFlags(Ra, value, result);
        Ra = (byte)result;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdcB() => Adc(Rb);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdcC() => Adc(Rc);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdcD() => Adc(Rd);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdcE() => Adc(Re);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdcH() => Adc(Rh);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdcL() => Adc(Rl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdcA() => Adc(Ra);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdcM()
    {
        var value = _mmu.Read(Rhl);
        var carry = (Flags & CpuFlags.C) != 0 ? 1 : 0;
        var result = Ra + value + carry;
        Flags = ComputeAddFlags(Ra, value, result, carry);
        Ra = (byte)result;
        return 7;
    }
}
