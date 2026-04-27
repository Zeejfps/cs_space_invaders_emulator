using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core.Tests;

public class CpuStackTests : CpuTestBase
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

        Mmu.Write(initialState.Pc, opcode);
        Mmu.Write(stackAddr, 0x30);
        Mmu.Write((ushort)(stackAddr + 1), 0x20);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Sp = (ushort)(stackAddr + 2);
        expectedState.WriteRegPair(dst, 0x2030);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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

        Mmu.Write(initialState.Pc, opcode);
        Mmu.Write(stackAddr, (byte)CpuFlags.Z);
        Mmu.Write((ushort)(stackAddr + 1), 0xAB);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Sp = (ushort)(stackAddr + 2);
        expectedState.Flags = CpuFlags.Z;
        expectedState.Ra = 0xAB;

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
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

        Mmu.Write(initialState.Pc, opcode);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Sp = (ushort)(stackAddr - 2);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
        Assert.Equal(0x30, Mmu.Read((ushort)(stackAddr - 2)));
        Assert.Equal(0x20, Mmu.Read((ushort)(stackAddr - 1)));
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

        Mmu.Write(initialState.Pc, opcode);

        Cpu.WriteState(initialState);
        var cycles = Cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);
        expectedState.Sp = (ushort)(stackAddr - 2);

        Assert.Equal(11, cycles);
        Assert.Equal(expectedState, Cpu.ReadState());
        Assert.Equal((byte)CpuFlags.Z, Mmu.Read((ushort)(stackAddr - 2)));
        Assert.Equal(0xAB, Mmu.Read((ushort)(stackAddr - 1)));
    }
}
