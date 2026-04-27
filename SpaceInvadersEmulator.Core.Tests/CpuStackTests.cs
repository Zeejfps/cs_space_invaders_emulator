using static SpaceInvadersEmulator.Core.Tests.CpuTestHelper;

namespace SpaceInvadersEmulator.Core.Tests;

public class CpuStackTests
{
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
            Flags = CpuFlags.All
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
            Flags = CpuFlags.All
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
            Flags = CpuFlags.All
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
}
