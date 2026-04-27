using static SpaceInvadersEmulator.Core.Tests.CpuTestHelper;

namespace SpaceInvadersEmulator.Core.Tests;

public class CpuArithmeticTests
{
    [Theory]
    [InlineData(0x80, Reg.B, 0x05, 0x15)] // ADD B
    [InlineData(0x81, Reg.C, 0x05, 0x15)] // ADD C
    [InlineData(0x82, Reg.D, 0x05, 0x15)] // ADD D
    [InlineData(0x83, Reg.E, 0x05, 0x15)] // ADD E
    [InlineData(0x84, Reg.H, 0x05, 0x15)] // ADD H
    [InlineData(0x85, Reg.L, 0x05, 0x15)] // ADD L
    [InlineData(0x87, Reg.A, 0x10, 0x20)] // ADD A
    public void TestAddRegister(byte opcode, Reg srcReg, byte srcVal, byte expectedA)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x10 };
        initialState.WriteReg(srcReg, srcVal);

        var mmu = new Mmu();
        mmu.Write(0x00, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestAddM()
    {
        ushort addr = 0x2000;
        var initialState = new CpuState { Pc = 0x00, Ra = 0x10 };
        initialState.WriteRegPair(Reg.H, addr);

        var mmu = new Mmu();
        mmu.Write(0x00, 0x86);
        mmu.Write(addr, 0x05);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = 0x15;
        expectedState.IncrementPcBy(1);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x01, 0x01, 0x02, CpuFlags.None)]                                  // no flags
    [InlineData(0xFF, 0x01, 0x00, CpuFlags.Z | CpuFlags.P | CpuFlags.C | CpuFlags.A)] // carry to zero
    [InlineData(0x7F, 0x01, 0x80, CpuFlags.S | CpuFlags.A)]                        // sign + aux carry
    [InlineData(0x01, 0x02, 0x03, CpuFlags.P)]                                     // parity only
    [InlineData(0xF0, 0x10, 0x00, CpuFlags.Z | CpuFlags.P | CpuFlags.C)]           // carry+zero, no aux carry
    [InlineData(0x70, 0x10, 0x80, CpuFlags.S)]                                     // sign only
    [InlineData(0x08, 0x08, 0x10, CpuFlags.A)]                                     // aux carry only
    public void TestAddFlags(byte a, byte b, byte expectedResult, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a, Rb = b };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x80); // ADD B

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedResult;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x88, Reg.B, 0x05, 0x16, CpuFlags.None)] // ADC B
    [InlineData(0x89, Reg.C, 0x05, 0x16, CpuFlags.None)] // ADC C
    [InlineData(0x8A, Reg.D, 0x05, 0x16, CpuFlags.None)] // ADC D
    [InlineData(0x8B, Reg.E, 0x05, 0x16, CpuFlags.None)] // ADC E
    [InlineData(0x8C, Reg.H, 0x05, 0x16, CpuFlags.None)] // ADC H
    [InlineData(0x8D, Reg.L, 0x05, 0x16, CpuFlags.None)] // ADC L
    [InlineData(0x8F, Reg.A, 0x10, 0x21, CpuFlags.P)]    // ADC A: 0x10 + 0x10 + 1 = 0x21
    public void TestAdcRegister(byte opcode, Reg srcReg, byte srcVal, byte expectedA, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x10, Flags = CpuFlags.C };
        initialState.WriteReg(srcReg, srcVal);

        var mmu = new Mmu();
        mmu.Write(0x00, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestAdcM()
    {
        ushort addr = 0x2000;
        var initialState = new CpuState { Pc = 0x00, Ra = 0x10, Flags = CpuFlags.C };
        initialState.WriteRegPair(Reg.H, addr);

        var mmu = new Mmu();
        mmu.Write(0x00, 0x8E);
        mmu.Write(addr, 0x05);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = 0x16;
        expectedState.Flags = CpuFlags.None;
        expectedState.IncrementPcBy(1);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(CpuFlags.None, 0x10, 0x05, 0x15, CpuFlags.None)]                                  // carry=0: no effect
    [InlineData(CpuFlags.C,    0x10, 0x05, 0x16, CpuFlags.None)]                                  // carry=1: adds 1 to result
    [InlineData(CpuFlags.C,    0xFF, 0x00, 0x00, CpuFlags.Z | CpuFlags.P | CpuFlags.C | CpuFlags.A)] // carry causes overflow
    [InlineData(CpuFlags.None, 0xFF, 0x00, 0xFF, CpuFlags.S | CpuFlags.P)]                        // no carry, no overflow
    public void TestAdcFlags(CpuFlags initialFlags, byte a, byte b, byte expectedResult, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a, Rb = b, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x88); // ADC B

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedResult;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x98, Reg.B, 0x10, 0x10, CpuFlags.None)] // SBB B
    [InlineData(0x99, Reg.C, 0x10, 0x10, CpuFlags.None)] // SBB C
    [InlineData(0x9A, Reg.D, 0x10, 0x10, CpuFlags.None)] // SBB D
    [InlineData(0x9B, Reg.E, 0x10, 0x10, CpuFlags.None)] // SBB E
    [InlineData(0x9C, Reg.H, 0x10, 0x10, CpuFlags.None)] // SBB H
    [InlineData(0x9D, Reg.L, 0x10, 0x10, CpuFlags.None)] // SBB L
    public void TestSbbRegister(byte opcode, Reg srcReg, byte srcVal, byte expectedA, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x21, Flags = CpuFlags.C };
        initialState.WriteReg(srcReg, srcVal);

        var mmu = new Mmu();
        mmu.Write(0x00, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestSbbA()
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x10, Flags = CpuFlags.C };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x9F); // SBB A

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = 0xFF;
        expectedState.Flags = CpuFlags.S | CpuFlags.P | CpuFlags.C | CpuFlags.A;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestSbbM()
    {
        ushort addr = 0x2000;
        var initialState = new CpuState { Pc = 0x00, Ra = 0x21, Flags = CpuFlags.C };
        initialState.WriteRegPair(Reg.H, addr);

        var mmu = new Mmu();
        mmu.Write(0x00, 0x9E); // SBB M
        mmu.Write(addr, 0x10);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = 0x10;
        expectedState.Flags = CpuFlags.None;
        expectedState.IncrementPcBy(1);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(CpuFlags.None, 0x11, 0x01, 0x10, CpuFlags.None)]                                        // carry=0: same as SUB
    [InlineData(CpuFlags.C,    0x12, 0x01, 0x10, CpuFlags.None)]                                        // carry=1: subtracts extra 1
    [InlineData(CpuFlags.C,    0x10, 0x00, 0x0F, CpuFlags.P | CpuFlags.A)]                              // carry causes aux borrow
    [InlineData(CpuFlags.C,    0x00, 0x00, 0xFF, CpuFlags.S | CpuFlags.P | CpuFlags.C | CpuFlags.A)]   // carry causes borrow
    [InlineData(CpuFlags.None, 0x00, 0x01, 0xFF, CpuFlags.S | CpuFlags.P | CpuFlags.C | CpuFlags.A)]   // borrow from value
    public void TestSbbFlags(CpuFlags initialFlags, byte a, byte b, byte expectedResult, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a, Rb = b, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x98); // SBB B

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedResult;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x90, Reg.B, 0x05, 0x10)] // SUB B
    [InlineData(0x91, Reg.C, 0x05, 0x10)] // SUB C
    [InlineData(0x92, Reg.D, 0x05, 0x10)] // SUB D
    [InlineData(0x93, Reg.E, 0x05, 0x10)] // SUB E
    [InlineData(0x94, Reg.H, 0x05, 0x10)] // SUB H
    [InlineData(0x95, Reg.L, 0x05, 0x10)] // SUB L
    public void TestSubRegister(byte opcode, Reg srcReg, byte srcVal, byte expectedA)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x15 };
        initialState.WriteReg(srcReg, srcVal);

        var mmu = new Mmu();
        mmu.Write(0x00, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestSubA()
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x10 };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x97); // SUB A

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = 0x00;
        expectedState.Flags = CpuFlags.Z | CpuFlags.P;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestSubM()
    {
        ushort addr = 0x2000;
        var initialState = new CpuState { Pc = 0x00, Ra = 0x15 };
        initialState.WriteRegPair(Reg.H, addr);

        var mmu = new Mmu();
        mmu.Write(0x00, 0x96); // SUB M
        mmu.Write(addr, 0x05);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = 0x10;
        expectedState.IncrementPcBy(1);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x11, 0x01, 0x10, CpuFlags.None)]                                        // no flags
    [InlineData(0x10, 0x05, 0x0B, CpuFlags.A)]                                           // aux borrow
    [InlineData(0x10, 0x10, 0x00, CpuFlags.Z | CpuFlags.P)]                              // zero+parity
    [InlineData(0x05, 0x10, 0xF5, CpuFlags.S | CpuFlags.P | CpuFlags.C)]                // borrow+sign+parity
    [InlineData(0x00, 0x01, 0xFF, CpuFlags.S | CpuFlags.P | CpuFlags.C | CpuFlags.A)]  // all flags
    public void TestSubFlags(byte a, byte b, byte expectedResult, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a, Rb = b };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x90); // SUB B

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedResult;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }
}
