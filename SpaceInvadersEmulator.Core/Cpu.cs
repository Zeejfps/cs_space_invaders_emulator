namespace SpaceInvadersEmulator.Core;

public sealed class Cpu
{
    public CpuFlags Flags { get; set; }
    public int Pc { get; set; }
    public byte Sp { get; set; }
    public byte Ra { get; set; }
    public byte Rb { get; set; }
    public byte Rc { get; set; }
    public byte Rd { get; set; }
    public byte Re { get; set; }
    public byte Rh { get; set; }
    public byte Rl { get; set; }

    public Cpu(Mmu mmu)
    {
        
    }

    public int Step()
    {
        Pc++;
        return 4;
    }
}