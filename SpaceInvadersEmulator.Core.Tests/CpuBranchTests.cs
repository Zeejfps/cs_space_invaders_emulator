using static SpaceInvadersEmulator.Core.Tests.CpuTestHelper;

namespace SpaceInvadersEmulator.Core.Tests;

public class CpuBranchTests
{
    [Theory]
    [InlineData(0xC0, CpuFlags.S | CpuFlags.C | CpuFlags.P | CpuFlags.A, true)]               // RNZ taken
    [InlineData(0xC0, CpuFlags.All, false)] // RNZ not taken
    [InlineData(0xD0, CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A, true)]               // RNC taken
    [InlineData(0xD0, CpuFlags.All, false)] // RNC not taken
    [InlineData(0xE0, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.A, true)]               // RPO taken
    [InlineData(0xE0, CpuFlags.All, false)] // RPO not taken
    [InlineData(0xF0, CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A, true)]               // RP taken
    [InlineData(0xF0, CpuFlags.All, false)] // RP not taken
    [InlineData(0xC8, CpuFlags.All, true)]  // RZ taken
    [InlineData(0xC8, CpuFlags.S | CpuFlags.C | CpuFlags.P | CpuFlags.A, false)]               // RZ not taken
    [InlineData(0xD8, CpuFlags.All, true)]  // RC taken
    [InlineData(0xD8, CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A, false)]               // RC not taken
    [InlineData(0xE8, CpuFlags.All, true)]  // RPE taken
    [InlineData(0xE8, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.A, false)]               // RPE not taken
    [InlineData(0xF8, CpuFlags.All, true)]  // RM taken
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

    [Theory]
    [InlineData(0xC2, CpuFlags.S | CpuFlags.C | CpuFlags.P | CpuFlags.A, true)]               // JNZ taken
    [InlineData(0xC2, CpuFlags.All, false)] // JNZ not taken
    [InlineData(0xD2, CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A, true)]               // JNC taken
    [InlineData(0xD2, CpuFlags.All, false)] // JNC not taken
    [InlineData(0xE2, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.A, true)]               // JPO taken
    [InlineData(0xE2, CpuFlags.All, false)] // JPO not taken
    [InlineData(0xF2, CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A, true)]               // JP taken
    [InlineData(0xF2, CpuFlags.All, false)] // JP not taken
    [InlineData(0xCA, CpuFlags.All, true)]                                                     // JZ taken
    [InlineData(0xCA, CpuFlags.S | CpuFlags.C | CpuFlags.P | CpuFlags.A, false)]              // JZ not taken
    [InlineData(0xDA, CpuFlags.All, true)]                                                     // JC taken
    [InlineData(0xDA, CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A, false)]              // JC not taken
    [InlineData(0xEA, CpuFlags.All, true)]                                                     // JPE taken
    [InlineData(0xEA, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.A, false)]              // JPE not taken
    [InlineData(0xFA, CpuFlags.All, true)]                                                     // JM taken
    [InlineData(0xFA, CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A, false)]              // JM not taken
    public void TestConditionalJump(byte opcode, CpuFlags flags, bool taken)
    {
        var initialState = new CpuState
        {
            Pc = 0x10,
            Flags = flags
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        if (taken)
            expectedState.Pc = 0x2030;
        else
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
            Flags = CpuFlags.All
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

    [Theory]
    [InlineData(0xC4, CpuFlags.S | CpuFlags.C | CpuFlags.P | CpuFlags.A, true)]               // CNZ taken
    [InlineData(0xC4, CpuFlags.All, false)] // CNZ not taken
    [InlineData(0xD4, CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A, true)]               // CNC taken
    [InlineData(0xD4, CpuFlags.All, false)] // CNC not taken
    [InlineData(0xE4, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.A, true)]               // CPO taken
    [InlineData(0xE4, CpuFlags.All, false)] // CPO not taken
    [InlineData(0xF4, CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A, true)]               // CP taken
    [InlineData(0xF4, CpuFlags.All, false)] // CP not taken
    [InlineData(0xCC, CpuFlags.All, true)]                                                     // CZ taken
    [InlineData(0xCC, CpuFlags.S | CpuFlags.C | CpuFlags.P | CpuFlags.A, false)]              // CZ not taken
    [InlineData(0xDC, CpuFlags.All, true)]                                                     // CC taken
    [InlineData(0xDC, CpuFlags.S | CpuFlags.Z | CpuFlags.P | CpuFlags.A, false)]              // CC not taken
    [InlineData(0xEC, CpuFlags.All, true)]                                                     // CPE taken
    [InlineData(0xEC, CpuFlags.S | CpuFlags.Z | CpuFlags.C | CpuFlags.A, false)]              // CPE not taken
    [InlineData(0xFC, CpuFlags.All, true)]                                                     // CM taken
    [InlineData(0xFC, CpuFlags.Z | CpuFlags.C | CpuFlags.P | CpuFlags.A, false)]              // CM not taken
    public void TestConditionalCall(byte opcode, CpuFlags flags, bool taken)
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
        mmu.Write((ushort)(initialState.Pc + 1), 0x30);
        mmu.Write((ushort)(initialState.Pc + 2), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        if (taken)
        {
            expectedState.Pc = 0x2030;
            expectedState.Sp = (ushort)(stackAddr - 2);
            Assert.Equal(17, cycles);
        }
        else
        {
            expectedState.IncrementPcBy(3);
            Assert.Equal(11, cycles);
        }

        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
        if (taken)
        {
            Assert.Equal(0x13, mmu.Read((ushort)(stackAddr - 2)));
            Assert.Equal(0x00, mmu.Read((ushort)(stackAddr - 1)));
        }
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
            Flags = CpuFlags.All
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

    [Fact]
    public void TestRet()
    {
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = CpuFlags.All
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, 0xC9);
        mmu.Write(stackAddr, 0x30);
        mmu.Write((ushort)(stackAddr + 1), 0x20);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;
        expectedState.Sp = (ushort)(stackAddr + 2);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestPchl()
    {
        var initialState = new CpuState { Pc = 0x10 };
        initialState.WriteRegPair(Reg.H, 0x2030);

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, 0xE9);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Pc = 0x2030;

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestCall()
    {
        ushort stackAddr = 0x2002;
        var initialState = new CpuState
        {
            Pc = 0x10,
            Sp = stackAddr,
            Flags = CpuFlags.All
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, 0xCD);
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
}
