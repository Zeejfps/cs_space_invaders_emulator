using System.Runtime.CompilerServices;

namespace SpaceInvadersEmulator.Core.Intel8080;

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
    public bool InterruptEnabled { get; set; }
    public bool Halted { get; private set; }
    
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

    private bool _isInterruptPending;
    private byte _pendingInterruptOpcode;
    private int _enableInterruptsTimer;
    private readonly ICpuIO _io;
    private readonly IMmu _mmu;
    
    public Cpu(IMmu mmu, ICpuIO io)
    {
        _mmu = mmu;
        _io = io;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public int Step()
    {
        if (_enableInterruptsTimer > 0)
        {
            _enableInterruptsTimer--;
            if (_enableInterruptsTimer == 0)
                InterruptEnabled = true;
        }

        if (InterruptEnabled && _isInterruptPending)
        {
            InterruptEnabled = false;
            Halted = false;
            _isInterruptPending = false;
            return Execute(_pendingInterruptOpcode);
        }

        if (Halted)
            return 4;

        var opcode = Fetch();
        return Execute(opcode);
    }

    public void Interrupt(byte opcode)
    {
        _isInterruptPending = true;
        _pendingInterruptOpcode = opcode;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private int Execute(byte opcode) => opcode switch
    {
        // NOP (primary + undocumented aliases)
        0x00 => Nop(),
        0x08 => Nop(),
        0x10 => Nop(),
        0x18 => Nop(),
        0x20 => Nop(),
        0x28 => Nop(),
        0x30 => Nop(),
        0x38 => Nop(),

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
        0x76 => Hlt(),
        0x77 => MoveMa(),

        // Load
        0x0A => LdAb(),
        0x1A => LdAd(),
        0x2A => Lhld(),
        0x3A => LdA(),

        // Store
        0x02 => StAb(),
        0x12 => StAd(),
        0x22 => Shld(),
        0x32 => StA(),

        // Load register pair immediate
        0x01 => LxiB(),
        0x11 => LxiD(),
        0x21 => LxiH(),
        0x31 => LxiSp(),

        // DAD
        0x09 => DadB(),
        0x19 => DadD(),
        0x29 => DadH(),
        0x39 => DadSp(),

        // INX
        0x03 => InxB(),
        0x13 => InxD(),
        0x23 => InxH(),
        0x33 => InxSp(),

        // DCX
        0x0B => DcxB(),
        0x1B => DcxD(),
        0x2B => DcxH(),
        0x3B => DcxSp(),

        // Stack operations
        0xC1 => PopB(),
        0xD1 => PopD(),
        0xE1 => PopH(),
        0xF1 => PopPsw(),

        0xC5 => PushB(),
        0xD5 => PushD(),
        0xE5 => PushH(),
        0xF5 => PushPsw(),

        // Conditional returns
        0xC0 => Rnz(),
        0xC8 => Rz(),
        0xD0 => Rnc(),
        0xD8 => Rcy(),
        0xE0 => Rpo(),
        0xE8 => Rpe(),
        0xF0 => Rp(),
        0xF8 => Rm(),

        // ADD
        0x80 => AddB(),
        0x81 => AddC(),
        0x82 => AddD(),
        0x83 => AddE(),
        0x84 => AddH(),
        0x85 => AddL(),
        0x86 => AddM(),
        0x87 => AddA(),

        // ADC
        0x88 => AdcB(),
        0x89 => AdcC(),
        0x8A => AdcD(),
        0x8B => AdcE(),
        0x8C => AdcH(),
        0x8D => AdcL(),
        0x8E => AdcM(),
        0x8F => AdcA(),

        // SUB
        0x90 => SubB(),
        0x91 => SubC(),
        0x92 => SubD(),
        0x93 => SubE(),
        0x94 => SubH(),
        0x95 => SubL(),
        0x96 => SubM(),
        0x97 => SubA(),

        // SBB
        0x98 => SbbB(),
        0x99 => SbbC(),
        0x9A => SbbD(),
        0x9B => SbbE(),
        0x9C => SbbH(),
        0x9D => SbbL(),
        0x9E => SbbM(),
        0x9F => SbbA(),

        // ANA
        0xA0 => AnaB(),
        0xA1 => AnaC(),
        0xA2 => AnaD(),
        0xA3 => AnaE(),
        0xA4 => AnaH(),
        0xA5 => AnaL(),
        0xA6 => AnaM(),
        0xA7 => AnaA(),

        // XRA
        0xA8 => XraB(),
        0xA9 => XraC(),
        0xAA => XraD(),
        0xAB => XraE(),
        0xAC => XraH(),
        0xAD => XraL(),
        0xAE => XraM(),
        0xAF => XraA(),

        // ORA
        0xB0 => OraB(),
        0xB1 => OraC(),
        0xB2 => OraD(),
        0xB3 => OraE(),
        0xB4 => OraH(),
        0xB5 => OraL(),
        0xB6 => OraM(),
        0xB7 => OraA(),

        // CMP
        0xB8 => CmpB(),
        0xB9 => CmpC(),
        0xBA => CmpD(),
        0xBB => CmpE(),
        0xBC => CmpH(),
        0xBD => CmpL(),
        0xBE => CmpM(),
        0xBF => CmpA(),

        // Unconditional return, call, pchl
        0xC9 => Ret(),
        0xD9 => Ret(),  // undocumented alias
        0xCD => Call(),
        0xDD => Call(), // undocumented alias
        0xED => Call(), // undocumented alias
        0xFD => Call(), // undocumented alias
        0xE9 => Pchl(),

        // Jumps
        0xC3 => Jmp(),
        0xCB => Jmp(),  // undocumented alias
        0xC2 => Jnz(),
        0xCA => Jz(),
        0xD2 => Jnc(),
        0xDA => Jc(),
        0xE2 => Jpo(),
        0xEA => Jpe(),
        0xF2 => Jp(),
        0xFA => Jm(),

        // Conditional calls
        0xC4 => Cnz(),
        0xCC => Cz(),
        0xD4 => Cnc(),
        0xDC => Cc(),
        0xE4 => Cpo(),
        0xEC => Cpe(),
        0xF4 => Cp(),
        0xFC => Cm(),

        // Restarts
        0xC7 => Rst0(),
        0xCF => Rst1(),
        0xD7 => Rst2(),
        0xDF => Rst3(),
        0xE7 => Rst4(),
        0xEF => Rst5(),
        0xF7 => Rst6(),
        0xFF => Rst7(),

        // I/O and interrupt control
        0xD3 => Out(),
        0xDB => In(),
        0xF3 => Di(),
        0xFB => Ei(),

        // Immediate arithmetic / logic
        0xC6 => Adi(),
        0xCE => Aci(),
        0xD6 => Sui(),
        0xDE => Sbi(),
        0xE6 => Ani(),
        0xEE => Xri(),
        0xF6 => Ori(),
        0xFE => Cpi(),

        // Rotate / special accumulator
        0x07 => Rlc(),
        0x0F => Rrc(),
        0x17 => Ral(),
        0x1F => Rar(),
        0x27 => Daa(),
        0x2F => Cma(),
        0x37 => Stc(),
        0x3F => Cmc(),

        // INR
        0x04 => InrB(),
        0x0C => InrC(),
        0x14 => InrD(),
        0x1C => InrE(),
        0x24 => InrH(),
        0x2C => InrL(),
        0x34 => InrM(),
        0x3C => InrA(),

        // DCR
        0x05 => DcrB(),
        0x0D => DcrC(),
        0x15 => DcrD(),
        0x1D => DcrE(),
        0x25 => DcrH(),
        0x2D => DcrL(),
        0x35 => DcrM(),
        0x3D => DcrA(),

        0xE3 => Xthl(),
        0xEB => Xchg(),
        0xF9 => Sphl(),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Nop()
    {
        return 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Hlt()
    {
        Halted = true;
        return 7;
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