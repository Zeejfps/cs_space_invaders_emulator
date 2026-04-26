using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    public CpuFlags Flags { get; set; }
    public ushort Pc { get; set; }
    public byte Sp { get; set; }
    public byte Ra { get; set; }
    public byte Rb { get; set; }
    public byte Rc { get; set; }
    public byte Rd { get; set; }
    public byte Re { get; set; }
    public byte Rh { get; set; }
    public byte Rl { get; set; }

    private readonly Mmu _mmu;
    
    public Cpu(Mmu mmu)
    {
        _mmu = mmu;
    }

    public int Step()
    {
        var opCode = FetchIns();
        Pc++;
        return opCode switch
        {
            0x00 => NoOp(),
            0x40 => MoveBb(),
            0x41 => MoveBc(),
            0x42 => MoveBd(),
            0x43 => MoveBe(),
            _ => 1
        };
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining)]
    private int NoOp()
    {
        return 4;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining)]
    private byte FetchIns()
    {
        return _mmu.Read(Pc);
    }
}