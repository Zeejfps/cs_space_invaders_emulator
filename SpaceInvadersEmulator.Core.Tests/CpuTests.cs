namespace SpaceInvadersEmulator.Core.Tests;

public class CpuTests
{
    private static readonly CpuFlags AllFlags = CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A;

    private static Cpu CreateCpu(Mmu mmu, CpuState state) => new(mmu)
    {
        Flags = state.Flags,
        Pc = state.Pc,
        Sp = state.Sp,
        Ra = state.Ra,
        Rb = state.Rb,
        Rc = state.Rc,
        Rd = state.Rd,
        Re = state.Re,
        Rh = state.Rh,
        Rl = state.Rl
    };

    [Fact]
    public void TestNoOp()
    {
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, 0x00);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        Assert.Equal(4, cycles);
        Assert.Equal(initialState with { Pc = (ushort)(initialState.Pc + 1) }, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x40, Reg.B, Reg.B)]
    [InlineData(0x41, Reg.B, Reg.C)]
    [InlineData(0x42, Reg.B, Reg.D)]
    [InlineData(0x43, Reg.B, Reg.E)]
    [InlineData(0x44, Reg.B, Reg.H)]
    [InlineData(0x45, Reg.B, Reg.L)]
    [InlineData(0x47, Reg.B, Reg.A)]
    [InlineData(0x48, Reg.C, Reg.B)]
    [InlineData(0x49, Reg.C, Reg.C)]
    [InlineData(0x4A, Reg.C, Reg.D)]
    [InlineData(0x4B, Reg.C, Reg.E)]
    [InlineData(0x4C, Reg.C, Reg.H)]
    [InlineData(0x4D, Reg.C, Reg.L)]
    [InlineData(0x4F, Reg.C, Reg.A)]
    [InlineData(0x50, Reg.D, Reg.B)]
    [InlineData(0x51, Reg.D, Reg.C)]
    [InlineData(0x52, Reg.D, Reg.D)]
    [InlineData(0x53, Reg.D, Reg.E)]
    [InlineData(0x54, Reg.D, Reg.H)]
    [InlineData(0x55, Reg.D, Reg.L)]
    [InlineData(0x57, Reg.D, Reg.A)]
    [InlineData(0x58, Reg.E, Reg.B)]
    [InlineData(0x59, Reg.E, Reg.C)]
    [InlineData(0x5A, Reg.E, Reg.D)]
    [InlineData(0x5B, Reg.E, Reg.E)]
    [InlineData(0x5C, Reg.E, Reg.H)]
    [InlineData(0x5D, Reg.E, Reg.L)]
    [InlineData(0x5F, Reg.E, Reg.A)]
    [InlineData(0x60, Reg.H, Reg.B)]
    [InlineData(0x61, Reg.H, Reg.C)]
    [InlineData(0x62, Reg.H, Reg.D)]
    [InlineData(0x63, Reg.H, Reg.E)]
    [InlineData(0x64, Reg.H, Reg.H)]
    [InlineData(0x65, Reg.H, Reg.L)]
    [InlineData(0x67, Reg.H, Reg.A)]
    [InlineData(0x68, Reg.L, Reg.B)]
    [InlineData(0x69, Reg.L, Reg.C)]
    [InlineData(0x6A, Reg.L, Reg.D)]
    [InlineData(0x6B, Reg.L, Reg.E)]
    [InlineData(0x6C, Reg.L, Reg.H)]
    [InlineData(0x6D, Reg.L, Reg.L)]
    [InlineData(0x6F, Reg.L, Reg.A)]
    [InlineData(0x78, Reg.A, Reg.B)]
    [InlineData(0x79, Reg.A, Reg.C)]
    [InlineData(0x7A, Reg.A, Reg.D)]
    [InlineData(0x7B, Reg.A, Reg.E)]
    [InlineData(0x7C, Reg.A, Reg.H)]
    [InlineData(0x7D, Reg.A, Reg.L)]
    [InlineData(0x7F, Reg.A, Reg.A)]
    public void TestMoveRr(byte opcode, Reg dst, Reg src)
    {
        var initialState = new CpuState { Pc = 0x10, Flags = AllFlags };
        initialState.WriteReg(dst, 0x11);
        initialState.WriteReg(src, 0x50);

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.WriteReg(dst, initialState.ReadReg(src));

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x46, Reg.B)]
    [InlineData(0x4E, Reg.C)]
    [InlineData(0x56, Reg.D)]
    [InlineData(0x5E, Reg.E)]
    [InlineData(0x66, Reg.H)]
    [InlineData(0x6E, Reg.L)]
    [InlineData(0x7E, Reg.A)]
    public void TestMoveRm(byte opcode, Reg dst)
    {
        var initialState = new CpuState
        {
            Pc = 0x10,
            Rh = 0x20,
            Rl = 0x30,
            Flags = AllFlags
        };
        initialState.WriteReg(dst, 0x11);
        var address = (ushort)((initialState.Rh << 8) | initialState.Rl);

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(address, 0x50);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.WriteReg(dst, 0x50);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x70, Reg.B)]
    [InlineData(0x71, Reg.C)]
    [InlineData(0x72, Reg.D)]
    [InlineData(0x73, Reg.E)]
    [InlineData(0x74, Reg.H)]
    [InlineData(0x75, Reg.L)]
    [InlineData(0x77, Reg.A)]
    public void TestMoveMr(byte opcode, Reg src)
    {
        var initialState = new CpuState
        {
            Pc = 0x10,
            Rh = 0x20,
            Rl = 0x30,
            Flags = AllFlags
        };
        // Keep the sentinel distinct from H and L so a row asserting "wrote H"
        // can't pass by accidentally writing the sentinel, and vice versa.
        if (src is not Reg.H and not Reg.L)
            initialState.WriteReg(src, 0xAB);

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        var expectedValueInMem = initialState.ReadReg(src);
        var valueInMem = mmu.Read(initialState.Rhl);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(expectedValueInMem, valueInMem);
    }

    [Theory]
    [InlineData(0x06, Reg.B)]
    [InlineData(0x0E, Reg.C)]
    [InlineData(0x16, Reg.D)]
    [InlineData(0x1E, Reg.E)]
    [InlineData(0x26, Reg.H)]
    [InlineData(0x2E, Reg.L)]
    [InlineData(0x3E, Reg.A)]
    public void TestMviR(byte opcode, Reg dst)
    {
        var instructionSize = 2;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0xAB);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);
        expectedState.WriteReg(dst, 0xAB);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestMviM()
    {
        byte opcode = 0x36;
        byte sentinel = 0xAB;
        var instructionSize = 2;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Rh = 0x20,
            Rl = 0x30,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), sentinel);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);

        var memValue = mmu.Read(expectedState.Rhl);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(sentinel, memValue);
    }

    [Theory]
    [InlineData(0x0A, Reg.B)]
    [InlineData(0x1A, Reg.D)]
    public void TestLdAx(byte opcode, Reg src)
    {
        byte sentinel = 0xAB;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = AllFlags
        };
        initialState.WriteRegPair(src, 0x2030);
        var address = initialState.ReadRegPair(src);

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(address, sentinel);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Ra = sentinel;

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestLdA()
    {
        byte opcode = 0x3A;
        byte sentinel = 0xAB;
        ushort address = 0x2030;
        var instructionSize = 3;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.WriteWord((ushort)(initialState.Pc + 1), address);
        mmu.Write(address, sentinel);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);
        expectedState.Ra = sentinel;

        Assert.Equal(13, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestStA()
    {
        byte opcode = 0x32;
        byte sentinel = 0xAB;
        ushort address = 0x2030;
        var instructionSize = 3;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Ra = sentinel,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.WriteWord((ushort)(initialState.Pc + 1), address);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);

        var memValue = mmu.Read(address);

        Assert.Equal(13, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(sentinel, memValue);
    }

    [Fact]
    public void TestShld()
    {
        byte opcode = 0x22;
        ushort address = 0x4050;
        var instructionSize = 3;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Rh = 0x20,
            Rl = 0x30,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.WriteWord((ushort)(initialState.Pc + 1), address);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);

        Assert.Equal(16, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(initialState.Rl, mmu.Read(address));
        Assert.Equal(initialState.Rh, mmu.Read((ushort)(address + 1)));
    }

    [Fact]
    public void TestLhld()
    {
        byte opcode = 0x2A;
        ushort address = 0x4050;
        var instructionSize = 3;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.WriteWord((ushort)(initialState.Pc + 1), address);
        mmu.Write(address, 0x30);
        mmu.Write((ushort)(address + 1), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);
        expectedState.Rh = 0x20;
        expectedState.Rl = 0x30;

        Assert.Equal(16, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x02, Reg.B)]
    [InlineData(0x12, Reg.D)]
    public void TestStAx(byte opcode, Reg src)
    {
        byte sentinel = 0xAB;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Ra = sentinel,
            Flags = AllFlags
        };
        initialState.WriteRegPair(src, 0x2030);
        var address = initialState.ReadRegPair(src);

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        var memValue = mmu.Read(address);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(sentinel, memValue);
    }

    [Theory]
    [InlineData(0x01, Reg.B)]
    [InlineData(0x11, Reg.D)]
    [InlineData(0x21, Reg.H)]
    [InlineData(0x31, Reg.Sp)]
    public void TestLxi(byte opcode, Reg dst)
    {
        ushort immediate = 0x2030;
        var instructionSize = 3;
        var initialState = new CpuState { Pc = 0x10, Flags = AllFlags };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.WriteWord((ushort)(initialState.Pc + 1), immediate);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);
        expectedState.WriteRegPair(dst, immediate);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xC1, Reg.B)]
    [InlineData(0xD1, Reg.D)]
    [InlineData(0xE1, Reg.H)]
    public void TestPopRp(byte opcode, Reg dst)
    {
        ushort stackAddr = 0x2000;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(stackAddr, 0x30);
        mmu.Write((ushort)(stackAddr + 1), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Sp = (ushort)(stackAddr + 2);
        expectedState.WriteRegPair(dst, 0x2030);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestPopPsw()
    {
        byte opcode = 0xF1;
        ushort stackAddr = 0x2000;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(stackAddr, (byte)CpuFlags.Z);
        mmu.Write((ushort)(stackAddr + 1), 0xAB);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Sp = (ushort)(stackAddr + 2);
        expectedState.Flags = CpuFlags.Z;
        expectedState.Ra = 0xAB;

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xC5, Reg.B)]
    [InlineData(0xD5, Reg.D)]
    [InlineData(0xE5, Reg.H)]
    public void TestPushRp(byte opcode, Reg src)
    {
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };
        initialState.WriteRegPair(src, 0x2030);

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Sp = (ushort)(stackAddr - 2);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(0x30, mmu.Read((ushort)(stackAddr - 2)));
        Assert.Equal(0x20, mmu.Read((ushort)(stackAddr - 1)));
    }

    [Fact]
    public void TestPushPsw()
    {
        byte opcode = 0xF5;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Ra = 0xAB,
            Flags = CpuFlags.Z
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Sp = (ushort)(stackAddr - 2);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal((byte)CpuFlags.Z, mmu.Read((ushort)(stackAddr - 2)));
        Assert.Equal(0xAB, mmu.Read((ushort)(stackAddr - 1)));
    }

    [Fact]
    public void TestXthl()
    {
        byte opcode = 0xE3;
        ushort stackAddr = 0x2000;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Rh = 0x20,
            Rl = 0x30,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(stackAddr, 0xAA);
        mmu.Write((ushort)(stackAddr + 1), 0xBB);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Rl = 0xAA;
        expectedState.Rh = 0xBB;

        Assert.Equal(18, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(0x30, mmu.Read(stackAddr));
        Assert.Equal(0x20, mmu.Read((ushort)(stackAddr + 1)));
    }

    [Fact]
    public void TestXchg()
    {
        byte opcode = 0xEB;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Rh = 0x20,
            Rl = 0x30,
            Rd = 0x40,
            Re = 0x50,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Rh = 0x40;
        expectedState.Rl = 0x50;
        expectedState.Rd = 0x20;
        expectedState.Re = 0x30;

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestSphl()
    {
        byte opcode = 0xF9;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Rh = 0x20,
            Rl = 0x30,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Sp = 0x2030;

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestRnzTaken()
    {
        byte opcode = 0xC0;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = CpuFlags.S | CpuFlags.C | CpuFlags.P | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(stackAddr, 0x30);
        mmu.Write((ushort)(stackAddr + 1), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;
        expectedState.Sp = (ushort)(stackAddr + 2);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestRnzNotTaken()
    {
        byte opcode = 0xC0;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestRncTaken()
    {
        byte opcode = 0xD0;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(stackAddr, 0x30);
        mmu.Write((ushort)(stackAddr + 1), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;
        expectedState.Sp = (ushort)(stackAddr + 2);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestRncNotTaken()
    {
        byte opcode = 0xD0;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestRpoTaken()
    {
        byte opcode = 0xE0;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(stackAddr, 0x30);
        mmu.Write((ushort)(stackAddr + 1), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;
        expectedState.Sp = (ushort)(stackAddr + 2);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestRpoNotTaken()
    {
        byte opcode = 0xE0;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestRpTaken()
    {
        byte opcode = 0xF0;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(stackAddr, 0x30);
        mmu.Write((ushort)(stackAddr + 1), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;
        expectedState.Sp = (ushort)(stackAddr + 2);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestRpNotTaken()
    {
        byte opcode = 0xF0;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xC8, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A, true)]  // RZ taken
    [InlineData(0xC8, CpuFlags.S | CpuFlags.C | CpuFlags.P | CpuFlags.A, false)]               // RZ not taken
    [InlineData(0xD8, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A, true)]  // RC taken
    [InlineData(0xD8, CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A, false)]               // RC not taken
    [InlineData(0xE8, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A, true)]  // RPE taken
    [InlineData(0xE8, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.A, false)]               // RPE not taken
    [InlineData(0xF8, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A, true)]  // RM taken
    [InlineData(0xF8, CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A, false)]               // RM not taken
    public void TestConditionalReturn(byte opcode, CpuFlags flags, bool taken)
    {
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = flags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(stackAddr, 0x30);
        mmu.Write((ushort)(stackAddr + 1), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        if (taken)
        {
            expectedState.Pc = 0x2030;
            expectedState.Sp = (ushort)(stackAddr + 2);
            Assert.Equal(11, cycles);
        }
        else
        {
            expectedState.IncrementPcBy(1);
            Assert.Equal(5, cycles);
        }

        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestJnzTaken()
    {
        byte opcode = 0xC2;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = CpuFlags.S | CpuFlags.C | CpuFlags.P | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestJnzNotTaken()
    {
        byte opcode = 0xC2;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(3);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestJncTaken()
    {
        byte opcode = 0xD2;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestJncNotTaken()
    {
        byte opcode = 0xD2;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(3);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestJpoTaken()
    {
        byte opcode = 0xE2;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestJpoNotTaken()
    {
        byte opcode = 0xE2;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(3);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestJpTaken()
    {
        byte opcode = 0xF2;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestJpNotTaken()
    {
        byte opcode = 0xF2;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(3);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestJmp()
    {
        byte opcode = 0xC3;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestCnzTaken()
    {
        byte opcode = 0xC4;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = CpuFlags.S | CpuFlags.C | CpuFlags.P | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;
        expectedState.Sp = (ushort)(stackAddr - 2);

        Assert.Equal(17, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(0x13, mmu.Read((ushort)(stackAddr - 2)));
        Assert.Equal(0x00, mmu.Read((ushort)(stackAddr - 1)));
    }

    [Fact]
    public void TestCnzNotTaken()
    {
        byte opcode = 0xC4;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(3);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestCncTaken()
    {
        byte opcode = 0xD4;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;
        expectedState.Sp = (ushort)(stackAddr - 2);

        Assert.Equal(17, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(0x13, mmu.Read((ushort)(stackAddr - 2)));
        Assert.Equal(0x00, mmu.Read((ushort)(stackAddr - 1)));
    }

    [Fact]
    public void TestCncNotTaken()
    {
        byte opcode = 0xD4;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(3);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestCpoTaken()
    {
        byte opcode = 0xE4;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;
        expectedState.Sp = (ushort)(stackAddr - 2);

        Assert.Equal(17, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(0x13, mmu.Read((ushort)(stackAddr - 2)));
        Assert.Equal(0x00, mmu.Read((ushort)(stackAddr - 1)));
    }

    [Fact]
    public void TestCpoNotTaken()
    {
        byte opcode = 0xE4;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(3);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestCpTaken()
    {
        byte opcode = 0xF4;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;
        expectedState.Sp = (ushort)(stackAddr - 2);

        Assert.Equal(17, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(0x13, mmu.Read((ushort)(stackAddr - 2)));
        Assert.Equal(0x00, mmu.Read((ushort)(stackAddr - 1)));
    }

    [Fact]
    public void TestCpNotTaken()
    {
        byte opcode = 0xF4;
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(3);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xC7, 0x0000)]
    [InlineData(0xCF, 0x0008)]
    [InlineData(0xD7, 0x0010)]
    [InlineData(0xDF, 0x0018)]
    [InlineData(0xE7, 0x0020)]
    [InlineData(0xEF, 0x0028)]
    [InlineData(0xF7, 0x0030)]
    [InlineData(0xFF, 0x0038)]
    public void TestRst(byte opcode, ushort target)
    {
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = AllFlags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = target;
        expectedState.Sp = (ushort)(stackAddr - 2);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(0x11, mmu.Read((ushort)(stackAddr - 2)));
        Assert.Equal(0x00, mmu.Read((ushort)(stackAddr - 1)));
    }
}
