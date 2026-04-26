namespace SpaceInvadersEmulator.Core.Tests;

public record struct CpuState
{
    public CpuFlags Flags;
    public byte Pc;
    public byte Sp;
    public byte Ra;
    public byte Rb;
    public byte Rc;
    public byte Rd;
    public byte Re;
    public byte Rh;
    public byte Rl;

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
}