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
}
