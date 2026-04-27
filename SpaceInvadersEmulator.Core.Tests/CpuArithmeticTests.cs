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

    [Theory]
    [InlineData(0x04, Reg.B, 0x01, 0x02)] // INR B
    [InlineData(0x0C, Reg.C, 0x01, 0x02)] // INR C
    [InlineData(0x14, Reg.D, 0x01, 0x02)] // INR D
    [InlineData(0x1C, Reg.E, 0x01, 0x02)] // INR E
    [InlineData(0x24, Reg.H, 0x01, 0x02)] // INR H
    [InlineData(0x2C, Reg.L, 0x01, 0x02)] // INR L
    [InlineData(0x3C, Reg.A, 0x01, 0x02)] // INR A
    public void TestInrRegister(byte opcode, Reg reg, byte initialVal, byte expectedVal)
    {
        var initialState = new CpuState { Pc = 0x00 };
        initialState.WriteReg(reg, initialVal);

        var mmu = new Mmu();
        mmu.Write(0x00, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.WriteReg(reg, expectedVal);
        expectedState.IncrementPcBy(1);

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestInrM()
    {
        ushort addr = 0x2000;
        var initialState = new CpuState { Pc = 0x00 };
        initialState.WriteRegPair(Reg.H, addr);

        var mmu = new Mmu();
        mmu.Write(0x00, 0x34); // INR M
        mmu.Write(addr, 0x01);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(0x02, mmu.Read(addr));
    }

    [Theory]
    [InlineData(0x01, 0x02, CpuFlags.None, CpuFlags.None)]                             // no flags
    [InlineData(0x02, 0x03, CpuFlags.None, CpuFlags.P)]                                // parity only
    [InlineData(0x0F, 0x10, CpuFlags.None, CpuFlags.A)]                                // aux carry only
    [InlineData(0x7F, 0x80, CpuFlags.None, CpuFlags.S | CpuFlags.A)]                  // sign + aux carry
    [InlineData(0xFF, 0x00, CpuFlags.None, CpuFlags.Z | CpuFlags.P | CpuFlags.A)]     // zero + parity + aux carry (carry NOT set)
    [InlineData(0x01, 0x02, CpuFlags.C,    CpuFlags.C)]                                // carry is preserved
    public void TestInrFlags(byte initial, byte expectedResult, CpuFlags initialFlags, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Rb = initial, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x04); // INR B

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Rb = expectedResult;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x05, Reg.B, 0x02, 0x01)] // DCR B
    [InlineData(0x0D, Reg.C, 0x02, 0x01)] // DCR C
    [InlineData(0x15, Reg.D, 0x02, 0x01)] // DCR D
    [InlineData(0x1D, Reg.E, 0x02, 0x01)] // DCR E
    [InlineData(0x25, Reg.H, 0x02, 0x01)] // DCR H
    [InlineData(0x2D, Reg.L, 0x02, 0x01)] // DCR L
    [InlineData(0x3D, Reg.A, 0x02, 0x01)] // DCR A
    public void TestDcrRegister(byte opcode, Reg reg, byte initialVal, byte expectedVal)
    {
        var initialState = new CpuState { Pc = 0x00 };
        initialState.WriteReg(reg, initialVal);

        var mmu = new Mmu();
        mmu.Write(0x00, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.WriteReg(reg, expectedVal);
        expectedState.IncrementPcBy(1);

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestDcrM()
    {
        ushort addr = 0x2000;
        var initialState = new CpuState { Pc = 0x00 };
        initialState.WriteRegPair(Reg.H, addr);

        var mmu = new Mmu();
        mmu.Write(0x00, 0x35); // DCR M
        mmu.Write(addr, 0x02);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        Assert.Equal(0x01, mmu.Read(addr));
    }

    [Theory]
    [InlineData(0x02, 0x01, CpuFlags.None, CpuFlags.None)]                             // no flags
    [InlineData(0x10, 0x0F, CpuFlags.None, CpuFlags.P | CpuFlags.A)]                  // parity + aux carry
    [InlineData(0x80, 0x7F, CpuFlags.None, CpuFlags.A)]                                // aux carry only
    [InlineData(0x01, 0x00, CpuFlags.None, CpuFlags.Z | CpuFlags.P)]                  // zero + parity
    [InlineData(0x00, 0xFF, CpuFlags.None, CpuFlags.S | CpuFlags.P | CpuFlags.A)]     // sign + parity + aux carry (carry NOT set)
    [InlineData(0x02, 0x01, CpuFlags.C,    CpuFlags.C)]                                // carry is preserved
    public void TestDcrFlags(byte initial, byte expectedResult, CpuFlags initialFlags, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Rb = initial, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x05); // DCR B

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Rb = expectedResult;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xB1, CpuFlags.None,             0x63, CpuFlags.C)]                         // bit7=1: wraps to bit0, C=1
    [InlineData(0x31, CpuFlags.None,             0x62, CpuFlags.None)]                      // bit7=0: C=0
    [InlineData(0x80, CpuFlags.None,             0x01, CpuFlags.C)]                         // only bit7: wraps, C=1
    [InlineData(0x01, CpuFlags.None,             0x02, CpuFlags.None)]                      // only bit0: shifts left, C=0
    [InlineData(0xB1, CpuFlags.S | CpuFlags.Z,  0x63, CpuFlags.S | CpuFlags.Z | CpuFlags.C)] // other flags preserved
    public void TestRlc(byte initial, CpuFlags initialFlags, byte expectedA, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = initial, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x07); // RLC

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xB1, CpuFlags.None,             0x62, CpuFlags.C)]                         // bit7=1, C in=0: bit0=0, C out=1
    [InlineData(0xB1, CpuFlags.C,                0x63, CpuFlags.C)]                         // bit7=1, C in=1: bit0=1, C out=1
    [InlineData(0x80, CpuFlags.C,                0x01, CpuFlags.C)]                         // bit7=1, C in=1: bit0=1, C out=1
    [InlineData(0x00, CpuFlags.C,                0x01, CpuFlags.None)]                      // bit7=0, C in=1: bit0=1, C out=0
    [InlineData(0x01, CpuFlags.S | CpuFlags.Z,  0x02, CpuFlags.S | CpuFlags.Z)]            // other flags preserved
    public void TestRal(byte initial, CpuFlags initialFlags, byte expectedA, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = initial, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x17); // RAL

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(CpuFlags.None,             CpuFlags.C)]                         // C gets set
    [InlineData(CpuFlags.C,                CpuFlags.C)]                         // already set, stays set
    [InlineData(CpuFlags.S | CpuFlags.Z,  CpuFlags.S | CpuFlags.Z | CpuFlags.C)] // other flags preserved
    public void TestStc(CpuFlags initialFlags, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x37); // STC

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xB1, CpuFlags.None,             0xD8, CpuFlags.C)]                         // bit0=1: wraps to bit7, C=1
    [InlineData(0xB2, CpuFlags.None,             0x59, CpuFlags.None)]                      // bit0=0: C=0
    [InlineData(0x01, CpuFlags.None,             0x80, CpuFlags.C)]                         // only bit0: wraps to bit7, C=1
    [InlineData(0x80, CpuFlags.None,             0x40, CpuFlags.None)]                      // only bit7: shifts right, C=0
    [InlineData(0xB1, CpuFlags.S | CpuFlags.Z,  0xD8, CpuFlags.S | CpuFlags.Z | CpuFlags.C)] // other flags preserved
    public void TestRrc(byte initial, CpuFlags initialFlags, byte expectedA, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = initial, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x0F); // RRC

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xB1, CpuFlags.None,             0x58, CpuFlags.C)]                         // bit0=1, C in=0: bit7=0, C out=1
    [InlineData(0xB1, CpuFlags.C,                0xD8, CpuFlags.C)]                         // bit0=1, C in=1: bit7=1, C out=1
    [InlineData(0x01, CpuFlags.C,                0x80, CpuFlags.C)]                         // bit0=1, C in=1: bit7=1, C out=1
    [InlineData(0x00, CpuFlags.C,                0x80, CpuFlags.None)]                      // bit0=0, C in=1: bit7=1, C out=0
    [InlineData(0x02, CpuFlags.S | CpuFlags.Z,  0x01, CpuFlags.S | CpuFlags.Z)]            // other flags preserved
    public void TestRar(byte initial, CpuFlags initialFlags, byte expectedA, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = initial, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x1F); // RAR

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xB1, 0x4E)]  // typical complement
    [InlineData(0x00, 0xFF)]  // zero → all ones
    [InlineData(0xFF, 0x00)]  // all ones → zero
    public void TestCma(byte initial, byte expectedA)
    {
        var initialFlags = CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A | CpuFlags.C;
        var initialState = new CpuState { Pc = 0x00, Ra = initial, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x2F); // CMA

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(CpuFlags.None,             CpuFlags.C)]                            // C=0 → C=1
    [InlineData(CpuFlags.C,                CpuFlags.None)]                         // C=1 → C=0
    [InlineData(CpuFlags.S | CpuFlags.Z,  CpuFlags.S | CpuFlags.Z | CpuFlags.C)] // other flags preserved when setting
    [InlineData(CpuFlags.S | CpuFlags.Z | CpuFlags.C, CpuFlags.S | CpuFlags.Z)]  // other flags preserved when clearing
    public void TestCmc(CpuFlags initialFlags, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x3F); // CMC

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x5C, CpuFlags.None,  0x62, CpuFlags.A)]                                        // low nibble > 9: add 6, AC set
    [InlineData(0xAE, CpuFlags.A,     0x14, CpuFlags.P | CpuFlags.A | CpuFlags.C)]              // AC set + high > 9: add 6 and 0x60
    [InlineData(0x0A, CpuFlags.None,  0x10, CpuFlags.A)]                                        // low = 0xA: add 6, AC set
    [InlineData(0x10, CpuFlags.A,     0x16, CpuFlags.None)]                                     // AC set but no carry from low correction
    [InlineData(0xA5, CpuFlags.None,  0x05, CpuFlags.P | CpuFlags.C)]                           // high > 9: add 0x60, C set
    [InlineData(0x05, CpuFlags.C,     0x65, CpuFlags.P | CpuFlags.C)]                           // C set: add 0x60 regardless
    [InlineData(0x9A, CpuFlags.None,  0x00, CpuFlags.Z | CpuFlags.P | CpuFlags.A | CpuFlags.C)] // both corrections, result = 0
    public void TestDaa(byte initial, CpuFlags initialFlags, byte expectedA, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = initial, Flags = initialFlags };

        var mmu = new Mmu();
        mmu.Write(0x00, 0x27); // DAA

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }
}
