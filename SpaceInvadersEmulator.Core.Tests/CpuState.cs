namespace SpaceInvadersEmulator.Core.Tests;

public enum Reg { A, B, C, D, E, H, L, Sp }

public record struct CpuState
{
    public CpuFlags Flags;
    public ushort Pc;
    public ushort Sp;
    public byte Ra;
    public byte Rb;
    public byte Rc;
    public byte Rd;
    public byte Re;
    public byte Rh;
    public byte Rl;
    
    public readonly ushort Rde => (ushort)((Rd << 8) | Re);
    public readonly ushort Rbc => (ushort)((Rb << 8) | Rc);
    public readonly ushort Rhl => (ushort)((Rh << 8) | Rl);

    public void IncrementPcBy(int n) => Pc = (ushort)(Pc + n);
    
    public static CpuState FromCpu(Cpu cpu) => new()
    {
        Flags = cpu.Flags,
        Pc = cpu.Pc,
        Sp = cpu.Sp,
        Ra = cpu.Ra,
        Rb = cpu.Rb,
        Rc = cpu.Rc,
        Rd = cpu.Rd,
        Re = cpu.Re,
        Rh = cpu.Rh,
        Rl = cpu.Rl
    };

    public void WriteRegPair(Reg r, ushort v)
    {
        switch (r)
        {
            case Reg.B: Rb = (byte)(v >> 8); Rc = (byte)(v & 0xFF); break;
            case Reg.D: Rd = (byte)(v >> 8); Re = (byte)(v & 0xFF); break;
            case Reg.H: Rh = (byte)(v >> 8); Rl = (byte)(v & 0xFF); break;
            case Reg.Sp: Sp = v; break;
            case Reg.A:
            case Reg.C:
            case Reg.E:
            case Reg.L:
            default: throw new ArgumentOutOfRangeException(nameof(r));
        }
    }

    public readonly ushort ReadRegPair(Reg r) => r switch
    {
        Reg.B => Rbc,
        Reg.D => Rde,
        Reg.H => Rhl,
        Reg.Sp => Sp,
        _ => throw new ArgumentOutOfRangeException(nameof(r))
    };

    public readonly byte ReadReg(Reg r) => r switch
    {
        Reg.A => Ra,
        Reg.B => Rb,
        Reg.C => Rc,
        Reg.D => Rd,
        Reg.E => Re,
        Reg.H => Rh,
        Reg.L => Rl,
        _ => throw new ArgumentOutOfRangeException(nameof(r))
    };

    public void WriteReg(Reg r, byte v)
    {
        switch (r)
        {
            case Reg.A: Ra = v; break;
            case Reg.B: Rb = v; break;
            case Reg.C: Rc = v; break;
            case Reg.D: Rd = v; break;
            case Reg.E: Re = v; break;
            case Reg.H: Rh = v; break;
            case Reg.L: Rl = v; break;
            default: throw new ArgumentOutOfRangeException(nameof(r));
        }
    }
}