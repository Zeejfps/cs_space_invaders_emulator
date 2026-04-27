using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rlc()
    {
        var carry = (Ra & 0x80) != 0;
        Ra = (byte)((Ra << 1) | (carry ? 1 : 0));
        Flags = carry ? (Flags | CpuFlags.C) : (Flags & ~CpuFlags.C);
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Ral()
    {
        var newCarry = (Ra & 0x80) != 0;
        var oldCarry = (Flags & CpuFlags.C) != 0;
        Ra = (byte)((Ra << 1) | (oldCarry ? 1 : 0));
        Flags = newCarry ? (Flags | CpuFlags.C) : (Flags & ~CpuFlags.C);
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rrc()
    {
        var carry = (Ra & 0x01) != 0;
        Ra = (byte)((Ra >> 1) | (carry ? 0x80 : 0));
        Flags = carry ? (Flags | CpuFlags.C) : (Flags & ~CpuFlags.C);
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Rar()
    {
        var newCarry = (Ra & 0x01) != 0;
        var oldCarry = (Flags & CpuFlags.C) != 0;
        Ra = (byte)((Ra >> 1) | (oldCarry ? 0x80 : 0));
        Flags = newCarry ? (Flags | CpuFlags.C) : (Flags & ~CpuFlags.C);
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Cma()
    {
        Ra = (byte)~Ra;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Cmc()
    {
        Flags ^= CpuFlags.C;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Stc()
    {
        Flags |= CpuFlags.C;
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Daa()
    {
        var a = Ra;
        var correction = 0;
        var newCarry = (Flags & CpuFlags.C) != 0;

        if ((Flags & CpuFlags.A) != 0 || (a & 0xF) > 9)
            correction = 0x06;

        if ((Flags & CpuFlags.C) != 0 || a > 0x99)
        {
            correction |= 0x60;
            newCarry = true;
        }

        var result = (byte)(a + correction);

        var flags = CpuFlags.None;
        if (result == 0) flags |= CpuFlags.Z;
        if ((result & 0x80) != 0) flags |= CpuFlags.S;
        if (Parity(result)) flags |= CpuFlags.P;
        if (newCarry) flags |= CpuFlags.C;
        if ((a & 0xF) + (correction & 0xF) > 0xF) flags |= CpuFlags.A;

        Flags = flags;
        Ra = result;
        return 4;
    }
}
