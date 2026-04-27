using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core.Tests;

public class CpuMovTests : CpuTestBase
{
    [Fact]
    public void TestNoOp()
    {
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = CpuFlags.All
        };

        Mmu.Write(initialState.Pc, 0x00);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        Assert.Equal(4, cycles);
        Assert.Equal(initialState with { Pc = (ushort)(initialState.Pc + 1) }, Cpu.ReadState());
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
        var initialState = new CpuState { Pc = 0x10, Flags = CpuFlags.All };
        initialState.WriteReg(dst, 0x11);
        initialState.WriteReg(src, 0x50);

        Mmu.Write(initialState.Pc, opcode);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.WriteReg(dst, initialState.ReadReg(src));

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };
        initialState.WriteReg(dst, 0x11);
        var address = initialState.Rhl;

        Mmu.Write(initialState.Pc, opcode);
        Mmu.Write(address, 0x50);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.WriteReg(dst, 0x50);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };
        // Keep the sentinel distinct from H and L so a row asserting "wrote H"
        // can't pass by accidentally writing the sentinel, and vice versa.
        if (src is not Reg.H and not Reg.L)
            initialState.WriteReg(src, 0xAB);

        Mmu.Write(initialState.Pc, opcode);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        var expectedValueInMem = initialState.ReadReg(src);
        var valueInMem = Mmu.Read(initialState.Rhl);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };

        Mmu.Write(initialState.Pc, opcode);
        Mmu.Write((ushort)(initialState.Pc + 1), 0xAB);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);
        expectedState.WriteReg(dst, 0xAB);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };

        Mmu.Write(initialState.Pc, opcode);
        Mmu.Write((ushort)(initialState.Pc + 1), sentinel);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);

        var memValue = Mmu.Read(expectedState.Rhl);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };
        initialState.WriteRegPair(src, 0x2030);
        var address = initialState.ReadRegPair(src);

        Mmu.Write(initialState.Pc, opcode);
        Mmu.Write(address, sentinel);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Ra = sentinel;

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };

        Mmu.Write(initialState.Pc, opcode);
        Mmu.WriteWord((ushort)(initialState.Pc + 1), address);
        Mmu.Write(address, sentinel);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);
        expectedState.Ra = sentinel;

        Assert.Equal(13, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };

        Mmu.Write(initialState.Pc, opcode);
        Mmu.WriteWord((ushort)(initialState.Pc + 1), address);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);

        var memValue = Mmu.Read(address);

        Assert.Equal(13, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };

        Mmu.Write(initialState.Pc, opcode);
        Mmu.WriteWord((ushort)(initialState.Pc + 1), address);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);

        Assert.Equal(16, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
        Assert.Equal(initialState.Rl, Mmu.Read(address));
        Assert.Equal(initialState.Rh, Mmu.Read((ushort)(address + 1)));
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
            Flags = CpuFlags.All
        };

        Mmu.Write(initialState.Pc, opcode);
        Mmu.WriteWord((ushort)(initialState.Pc + 1), address);
        Mmu.Write(address, 0x30);
        Mmu.Write((ushort)(address + 1), 0x20);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);
        expectedState.Rh = 0x20;
        expectedState.Rl = 0x30;

        Assert.Equal(16, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };
        initialState.WriteRegPair(src, 0x2030);
        var address = initialState.ReadRegPair(src);

        Mmu.Write(initialState.Pc, opcode);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        var memValue = Mmu.Read(address);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
        var initialState = new CpuState { Pc = 0x10, Flags = CpuFlags.All };

        Mmu.Write(initialState.Pc, opcode);
        Mmu.WriteWord((ushort)(initialState.Pc + 1), immediate);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(instructionSize);
        expectedState.WriteRegPair(dst, immediate);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };

        Mmu.Write(initialState.Pc, opcode);
        Mmu.Write(stackAddr, 0xAA);
        Mmu.Write((ushort)(stackAddr + 1), 0xBB);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Rl = 0xAA;
        expectedState.Rh = 0xBB;

        Assert.Equal(18, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
        Assert.Equal(0x30, Mmu.Read(stackAddr));
        Assert.Equal(0x20, Mmu.Read((ushort)(stackAddr + 1)));
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
            Flags = CpuFlags.All
        };

        Mmu.Write(initialState.Pc, opcode);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Rh = 0x40;
        expectedState.Rl = 0x50;
        expectedState.Rd = 0x20;
        expectedState.Re = 0x30;

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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
            Flags = CpuFlags.All
        };

        Mmu.Write(initialState.Pc, opcode);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Sp = 0x2030;

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
    }
}
