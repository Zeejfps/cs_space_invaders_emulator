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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static CpuFlags ComputeSubFlags(byte a, byte b, int result, int borrow = 0)
    {
        var flags = CpuFlags.None;
        var byteResult = (byte)result;
        if (byteResult == 0) flags |= CpuFlags.Z;
        if ((byteResult & 0x80) != 0) flags |= CpuFlags.S;
        if (Parity(byteResult)) flags |= CpuFlags.P;
        if (result < 0) flags |= CpuFlags.C;
        if ((a & 0xF) < (b & 0xF) + borrow) flags |= CpuFlags.A;
        return flags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Sub(byte value)
    {
        var result = Ra - value;
        Flags = ComputeSubFlags(Ra, value, result);
        Ra = (byte)result;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SubB() => Sub(Rb);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SubC() => Sub(Rc);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SubD() => Sub(Rd);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SubE() => Sub(Re);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SubH() => Sub(Rh);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SubL() => Sub(Rl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SubA() => Sub(Ra);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SubM()
    {
        var value = _mmu.Read(Rhl);
        var result = Ra - value;
        Flags = ComputeSubFlags(Ra, value, result);
        Ra = (byte)result;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Sbb(byte value)
    {
        var borrow = (Flags & CpuFlags.C) != 0 ? 1 : 0;
        var result = Ra - value - borrow;
        Flags = ComputeSubFlags(Ra, value, result, borrow);
        Ra = (byte)result;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SbbB() => Sbb(Rb);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SbbC() => Sbb(Rc);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SbbD() => Sbb(Rd);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SbbE() => Sbb(Re);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SbbH() => Sbb(Rh);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SbbL() => Sbb(Rl);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SbbA() => Sbb(Ra);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int SbbM()
    {
        var value = _mmu.Read(Rhl);
        var borrow = (Flags & CpuFlags.C) != 0 ? 1 : 0;
        var result = Ra - value - borrow;
        Flags = ComputeSubFlags(Ra, value, result, borrow);
        Ra = (byte)result;
        return 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static CpuFlags ComputeInrDcrFlags(byte result, bool auxCarry, CpuFlags currentFlags)
    {
        var flags = CpuFlags.None;
        if (result == 0) flags |= CpuFlags.Z;
        if ((result & 0x80) != 0) flags |= CpuFlags.S;
        if (Parity(result)) flags |= CpuFlags.P;
        if (auxCarry) flags |= CpuFlags.A;
        return flags | (currentFlags & CpuFlags.C);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte Inr(byte value)
    {
        var result = (byte)(value + 1);
        Flags = ComputeInrDcrFlags(result, (value & 0xF) == 0xF, Flags);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InrB() { Rb = Inr(Rb); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InrC() { Rc = Inr(Rc); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InrD() { Rd = Inr(Rd); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InrE() { Re = Inr(Re); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InrH() { Rh = Inr(Rh); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InrL() { Rl = Inr(Rl); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InrA() { Ra = Inr(Ra); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int InrM()
    {
        _mmu.Write(Rhl, Inr(_mmu.Read(Rhl)));
        return 10;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte Dcr(byte value)
    {
        var result = (byte)(value - 1);
        Flags = ComputeInrDcrFlags(result, (value & 0xF) == 0x0, Flags);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcrB() { Rb = Dcr(Rb); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcrC() { Rc = Dcr(Rc); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcrD() { Rd = Dcr(Rd); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcrE() { Re = Dcr(Re); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcrH() { Rh = Dcr(Rh); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcrL() { Rl = Dcr(Rl); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcrA() { Ra = Dcr(Ra); return 5; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DcrM()
    {
        _mmu.Write(Rhl, Dcr(_mmu.Read(Rhl)));
        return 10;
    }
}
