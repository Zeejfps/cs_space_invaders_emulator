namespace SpaceInvadersEmulator.Core;

public sealed class Cpu
{
    public byte Flags { get; set; }
    public int Pc { get; set; }
    public int Sp { get; set; }
    public int Ra { get; set; }
    public int Rb { get; set; }
    public int Rc { get; set; }
    public int Rd { get; set; }
    public int Re { get; set; }
    public int Rh { get; set; }
    public int Rl { get; set; }

    public Cpu(Mmu mmu)
    {
        
    }

    public int Step()
    {
        Pc++;
        return 4;
    }
}