namespace SpaceInvadersEmulator.Core.Tests;

public enum Reg { A, B, C, D, E, H, L }

public record struct CpuState
{
    public CpuFlags Flags;
    public byte Pc;
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

    public void IncrementPcBy(int n) => Pc = (byte)(Pc + n);
    
    public static CpuState FromCpu(Cpu cpu) => new()
    {
        Flags = cpu.Flags,
        Pc = (byte)cpu.Pc,
        Sp = cpu.Sp,
        Ra = cpu.Ra,
        Rb = cpu.Rb,
        Rc = cpu.Rc,
        Rd = cpu.Rd,
        Re = cpu.Re,
        Rh = cpu.Rh,
        Rl = cpu.Rl
    };
    
    public readonly ushort ReadRegPair(Reg r) => r switch
    {
        Reg.B => Rbc,
        Reg.D => Rde,
        Reg.H => Rhl,
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