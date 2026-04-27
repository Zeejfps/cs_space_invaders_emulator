using static SpaceInvadersEmulator.Core.Tests.CpuTestHelper;

namespace SpaceInvadersEmulator.Core.Tests;

public class CpuLogicTests
{
    [Theory]
    [InlineData(0xA0, Reg.B, 0x15, 0x15)] // ANA B
    [InlineData(0xA1, Reg.C, 0x15, 0x15)] // ANA C
    [InlineData(0xA2, Reg.D, 0x15, 0x15)] // ANA D
    [InlineData(0xA3, Reg.E, 0x15, 0x15)] // ANA E
    [InlineData(0xA4, Reg.H, 0x15, 0x15)] // ANA H
    [InlineData(0xA5, Reg.L, 0x15, 0x15)] // ANA L
    [InlineData(0xA7, Reg.A, 0x15, 0x15)] // ANA A
    public void TestAnaRegister(byte opcode, Reg srcReg, byte srcVal, byte expectedA)
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
    public void TestAnaM()
    {
        ushort addr = 0x2000;
        var initialState = new CpuState { Pc = 0x00, Ra = 0x15 };
        initialState.WriteRegPair(Reg.H, addr);

        var mmu = new Mmu();
        mmu.Write(0x00, 0xA6); // ANA M
        mmu.Write(addr, 0x15);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = 0x15;
        expectedState.IncrementPcBy(1);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x11, 0x01, 0x01, CpuFlags.None)]                              // no flags
    [InlineData(0xF0, 0x0F, 0x00, CpuFlags.Z | CpuFlags.P | CpuFlags.A)]      // zero+parity+aux (bit 3 set in operands)
    [InlineData(0xFF, 0x80, 0x80, CpuFlags.S | CpuFlags.A)]                    // sign+aux
    [InlineData(0x0F, 0x0F, 0x0F, CpuFlags.P | CpuFlags.A)]                    // parity+aux
    [InlineData(0x08, 0x08, 0x08, CpuFlags.A)]                                 // aux carry only (bit 3 of both operands set)
    public void TestAnaFlags(byte a, byte b, byte expectedResult, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a, Rb = b };

        var mmu = new Mmu();
        mmu.Write(0x00, 0xA0); // ANA B

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
    [InlineData(0xA8, Reg.B, 0x07, 0x08)] // XRA B
    [InlineData(0xA9, Reg.C, 0x07, 0x08)] // XRA C
    [InlineData(0xAA, Reg.D, 0x07, 0x08)] // XRA D
    [InlineData(0xAB, Reg.E, 0x07, 0x08)] // XRA E
    [InlineData(0xAC, Reg.H, 0x07, 0x08)] // XRA H
    [InlineData(0xAD, Reg.L, 0x07, 0x08)] // XRA L
    public void TestXraRegister(byte opcode, Reg srcReg, byte srcVal, byte expectedA)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x0F };
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
    public void TestXraA()
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x10 };

        var mmu = new Mmu();
        mmu.Write(0x00, 0xAF); // XRA A

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
    public void TestXraM()
    {
        ushort addr = 0x2000;
        var initialState = new CpuState { Pc = 0x00, Ra = 0x0F };
        initialState.WriteRegPair(Reg.H, addr);

        var mmu = new Mmu();
        mmu.Write(0x00, 0xAE); // XRA M
        mmu.Write(addr, 0x07);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = 0x08;
        expectedState.IncrementPcBy(1);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x0F, 0x07, 0x08, CpuFlags.None)]                   // no flags
    [InlineData(0xFF, 0xFF, 0x00, CpuFlags.Z | CpuFlags.P)]         // zero+parity
    [InlineData(0xFF, 0x7F, 0x80, CpuFlags.S)]                      // sign only
    [InlineData(0x3C, 0x0F, 0x33, CpuFlags.P)]                      // parity only
    [InlineData(0x00, 0xFF, 0xFF, CpuFlags.S | CpuFlags.P)]         // sign+parity
    public void TestXraFlags(byte a, byte b, byte expectedResult, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a, Rb = b };

        var mmu = new Mmu();
        mmu.Write(0x00, 0xA8); // XRA B

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
    [InlineData(0xB0, Reg.B, 0x10, 0x10)] // ORA B
    [InlineData(0xB1, Reg.C, 0x10, 0x10)] // ORA C
    [InlineData(0xB2, Reg.D, 0x10, 0x10)] // ORA D
    [InlineData(0xB3, Reg.E, 0x10, 0x10)] // ORA E
    [InlineData(0xB4, Reg.H, 0x10, 0x10)] // ORA H
    [InlineData(0xB5, Reg.L, 0x10, 0x10)] // ORA L
    [InlineData(0xB7, Reg.A, 0x10, 0x10)] // ORA A
    public void TestOraRegister(byte opcode, Reg srcReg, byte srcVal, byte expectedA)
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
    public void TestOraM()
    {
        ushort addr = 0x2000;
        var initialState = new CpuState { Pc = 0x00, Ra = 0x10 };
        initialState.WriteRegPair(Reg.H, addr);

        var mmu = new Mmu();
        mmu.Write(0x00, 0xB6); // ORA M
        mmu.Write(addr, 0x10);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = 0x10;
        expectedState.IncrementPcBy(1);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x10, 0x10, 0x10, CpuFlags.None)]                   // no flags
    [InlineData(0xF0, 0x0F, 0xFF, CpuFlags.S | CpuFlags.P)]         // sign+parity
    [InlineData(0x00, 0x00, 0x00, CpuFlags.Z | CpuFlags.P)]         // zero
    [InlineData(0x80, 0x80, 0x80, CpuFlags.S)]                      // sign only
    [InlineData(0x01, 0x02, 0x03, CpuFlags.P)]                      // parity only
    public void TestOraFlags(byte a, byte b, byte expectedResult, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a, Rb = b };

        var mmu = new Mmu();
        mmu.Write(0x00, 0xB0); // ORA B

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
    [InlineData(0xB8, Reg.B, 0x01, CpuFlags.None)] // CMP B
    [InlineData(0xB9, Reg.C, 0x01, CpuFlags.None)] // CMP C
    [InlineData(0xBA, Reg.D, 0x01, CpuFlags.None)] // CMP D
    [InlineData(0xBB, Reg.E, 0x01, CpuFlags.None)] // CMP E
    [InlineData(0xBC, Reg.H, 0x01, CpuFlags.None)] // CMP H
    [InlineData(0xBD, Reg.L, 0x01, CpuFlags.None)] // CMP L
    public void TestCmpRegister(byte opcode, Reg srcReg, byte srcVal, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x11 };
        initialState.WriteReg(srcReg, srcVal);

        var mmu = new Mmu();
        mmu.Write(0x00, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestCmpA()
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x10 };

        var mmu = new Mmu();
        mmu.Write(0x00, 0xBF); // CMP A

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Flags = CpuFlags.Z | CpuFlags.P;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestCmpM()
    {
        ushort addr = 0x2000;
        var initialState = new CpuState { Pc = 0x00, Ra = 0x11 };
        initialState.WriteRegPair(Reg.H, addr);

        var mmu = new Mmu();
        mmu.Write(0x00, 0xBE); // CMP M
        mmu.Write(addr, 0x01);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Flags = CpuFlags.None;
        expectedState.IncrementPcBy(1);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x11, 0x01, CpuFlags.None)]                                        // no flags
    [InlineData(0x10, 0x05, CpuFlags.A)]                                           // aux borrow
    [InlineData(0x10, 0x10, CpuFlags.Z | CpuFlags.P)]                              // equal
    [InlineData(0x05, 0x10, CpuFlags.S | CpuFlags.P | CpuFlags.C)]                // less than
    [InlineData(0x00, 0x01, CpuFlags.S | CpuFlags.P | CpuFlags.C | CpuFlags.A)]  // all flags
    public void TestCmpFlags(byte a, byte b, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a, Rb = b };

        var mmu = new Mmu();
        mmu.Write(0x00, 0xB8); // CMP B

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xFF, 0xFF, 0x00, CpuFlags.Z | CpuFlags.P)]  // XOR with self: zero
    [InlineData(0xFF, 0x0F, 0xF0, CpuFlags.S | CpuFlags.P)]  // sign + parity
    [InlineData(0x80, 0x00, 0x80, CpuFlags.S)]               // sign only
    [InlineData(0x01, 0x02, 0x03, CpuFlags.P)]               // parity only
    public void TestXri(byte a, byte imm, byte expectedA, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a };

        var mmu = new Mmu();
        mmu.Write(0x00, 0xEE); // XRI
        mmu.Write(0x01, imm);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(2);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x11, 0x01, CpuFlags.None)]                                        // a > imm: no flags
    [InlineData(0x10, 0x05, CpuFlags.A)]                                           // aux borrow
    [InlineData(0x10, 0x10, CpuFlags.Z | CpuFlags.P)]                              // equal
    [InlineData(0x05, 0x10, CpuFlags.S | CpuFlags.P | CpuFlags.C)]                // less than
    [InlineData(0x00, 0x01, CpuFlags.S | CpuFlags.P | CpuFlags.C | CpuFlags.A)]  // all flags
    public void TestCpi(byte a, byte imm, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a };

        var mmu = new Mmu();
        mmu.Write(0x00, 0xFE); // CPI
        mmu.Write(0x01, imm);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(2);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0xFF, 0x0F, 0x0F, CpuFlags.P | CpuFlags.A)]              // bit3 in both operands: A set, 4 bits even: P set
    [InlineData(0xFF, 0xF0, 0xF0, CpuFlags.S | CpuFlags.P | CpuFlags.A)] // sign + parity + A
    [InlineData(0xF0, 0x00, 0x00, CpuFlags.Z | CpuFlags.P)]              // neither has bit3: A clear, zero
    [InlineData(0x0F, 0x08, 0x08, CpuFlags.A)]                           // bit3 in Ra: A set
    public void TestAni(byte a, byte imm, byte expectedA, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a };

        var mmu = new Mmu();
        mmu.Write(0x00, 0xE6); // ANI
        mmu.Write(0x01, imm);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(2);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x10, 0x05, 0x15, CpuFlags.None)]             // basic OR, no flags
    [InlineData(0xC0, 0x00, 0xC0, CpuFlags.S | CpuFlags.P)]  // sign + parity
    [InlineData(0x00, 0x00, 0x00, CpuFlags.Z | CpuFlags.P)]  // zero
    [InlineData(0x01, 0x02, 0x03, CpuFlags.P)]                // parity only
    public void TestOri(byte a, byte imm, byte expectedA, CpuFlags expectedFlags)
    {
        var initialState = new CpuState { Pc = 0x00, Ra = a };

        var mmu = new Mmu();
        mmu.Write(0x00, 0xF6); // ORI
        mmu.Write(0x01, imm);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = expectedA;
        expectedState.Flags = expectedFlags;
        expectedState.IncrementPcBy(2);

        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }
}
