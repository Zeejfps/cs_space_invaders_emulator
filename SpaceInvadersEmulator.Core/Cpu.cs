using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core;

public sealed partial class Cpu
{
    public CpuFlags Flags { get; set; }
    public ushort Pc { get; set; }
    public ushort Sp { get; set; }
    public byte Ra { get; set; }
    public byte Rb { get; set; }
    public byte Rc { get; set; }
    public byte Rd { get; set; }
    public byte Re { get; set; }
    public byte Rh { get; set; }
    public byte Rl { get; set; }

    private ushort Rbc
    {
        get => (ushort)((Rb << 8) | Rc);
        set { Rb = (byte)(value >> 8); Rc = (byte)(value & 0xFF); }
    }
    
    private ushort Rde
    {
        get => (ushort)((Rd << 8) | Re);
        set { Rd = (byte)(value >> 8); Re = (byte)(value & 0xFF); }
    }
    
    private ushort Rhl
    {
        get => (ushort)((Rh << 8) | Rl);
        set { Rh = (byte)(value >> 8); Rl = (byte)(value & 0xFF); }
    }

    private readonly Mmu _mmu;
    
    public Cpu(Mmu mmu)
    {
        _mmu = mmu;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public int Step()
    {
        var opcode = Fetch();
        return opcode switch
        {
            0x00 => NoOp(),

            // Mvi
            0x06 => MviB(),
            0x0E => MviC(),
            0x16 => MviD(),
            0x1E => MviE(),
            0x26 => MviH(),
            0x2E => MviL(),
            0x3E => MviA(),
            0x36 => MviM(),

            // Move Bx
            0x40 => MoveBb(),
            0x41 => MoveBc(),
            0x42 => MoveBd(),
            0x43 => MoveBe(),
            0x44 => MoveBh(),
            0x45 => MoveBl(),
            0x46 => MoveBm(),
            0x47 => MoveBa(),
            
            // Move Cx
            0x48 => MoveCb(),
            0x49 => MoveCc(),
            0x4A => MoveCd(),
            0x4B => MoveCe(),
            0x4C => MoveCh(),
            0x4D => MoveCl(),
            0x4E => MoveCm(),
            0x4F => MoveCa(),
            
            // Move Dx
            0x50 => MoveDb(),
            0x51 => MoveDc(),
            0x52 => MoveDd(),
            0x53 => MoveDe(),
            0x54 => MoveDh(),
            0x55 => MoveDl(),
            0x56 => MoveDm(),
            0x57 => MoveDa(),
            
            // Move Ex
            0x58 => MoveEb(),
            0x59 => MoveEc(),
            0x5A => MoveEd(),
            0x5B => MoveEe(),
            0x5C => MoveEh(),
            0x5D => MoveEl(),
            0x5E => MoveEm(),
            0x5F => MoveEa(),
            
            // Move Hx
            0x60 => MoveHb(),
            0x61 => MoveHc(),
            0x62 => MoveHd(),
            0x63 => MoveHe(),
            0x64 => MoveHh(),
            0x65 => MoveHl(),
            0x66 => MoveHm(),
            0x67 => MoveHa(),
            
            // Move Lx
            0x68 => MoveLb(),
            0x69 => MoveLc(),
            0x6A => MoveLd(),
            0x6B => MoveLe(),
            0x6C => MoveLh(),
            0x6D => MoveLl(),
            0x6E => MoveLm(),
            0x6F => MoveLa(),
            
            // Move Ax
            0x78 => MoveAb(),
            0x79 => MoveAc(),
            0x7A => MoveAd(),
            0x7B => MoveAe(),
            0x7C => MoveAh(),
            0x7D => MoveAl(),
            0x7E => MoveAm(),
            0x7F => MoveAa(),
            
            // Move Mx
            0x70 => MoveMb(),
            0x71 => MoveMc(),
            0x72 => MoveMd(),
            0x73 => MoveMe(),
            0x74 => MoveMh(),
            0x75 => MoveMl(),
            0x77 => MoveMa(),
            
            // Load
            0x0A => LdAb(),
            0x1A => LdAd(),
            0x3A => LdA(),
            
            // Store
            0x02 => StAb(),
            0x12 => StAd(),
            0x32 => StA(),

            // Load register pair immediate
            0x01 => LxiB(),
            0x11 => LxiD(),
            0x21 => LxiH(),
            0x31 => LxiSp(),
            
            _ => 1
        };
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining)]
    private int NoOp()
    {
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte Fetch()
    {
        return _mmu.Read(Pc++);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ushort FetchWord()
    {
        var lo = Fetch();
        var hi = Fetch();
        return (ushort)((hi << 8) | lo);
    }
}