using SpaceInvadersEmulator.Core.Intel8080;
using static SpaceInvadersEmulator.Core.Tests.CpuTestHelper;

namespace SpaceInvadersEmulator.Core.Tests;

public class CpuIoTests
{
    [Fact]
    public void TestDi()
    {
        var initialState = new CpuState { Pc = 0x00 };
        var mmu = new Mmu();
        mmu.Write(0x00, 0xF3); // DI

        var cpu = CreateCpu(mmu, initialState);
        cpu.InterruptEnabled = true;
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(1);

        Assert.Equal(4, cycles);
        Assert.False(cpu.InterruptEnabled);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestDiIsIdempotentWhenAlreadyDisabled()
    {
        var mmu = new Mmu();
        mmu.Write(0x00, 0xF3); // DI

        var cpu = CreateCpu(mmu, new CpuState { Pc = 0x00 });
        cpu.InterruptEnabled = false;
        cpu.Step();

        Assert.False(cpu.InterruptEnabled);
    }

    [Fact]
    public void TestEiDoesNotEnableImmediately()
    {
        var mmu = new Mmu();
        mmu.Write(0x00, 0xFB); // EI

        var cpu = CreateCpu(mmu, new CpuState { Pc = 0x00 });
        var cycles = cpu.Step();

        Assert.Equal(4, cycles);
        Assert.False(cpu.InterruptEnabled); // still disabled right after EI
    }

    [Fact]
    public void TestEiEnablesOnFollowingStep()
    {
        var mmu = new Mmu();
        mmu.Write(0x00, 0xFB); // EI
        mmu.Write(0x01, 0x00); // NOP

        var cpu = CreateCpu(mmu, new CpuState { Pc = 0x00 });
        cpu.Step(); // EI
        cpu.Step(); // next instruction — interrupts enabled at start of this step

        Assert.True(cpu.InterruptEnabled);
    }

    [Fact]
    public void TestIn()
    {
        var initialState = new CpuState { Pc = 0x00 };
        var mmu = new Mmu();
        mmu.Write(0x00, 0xDB); // IN
        mmu.Write(0x01, 0x42); // port number

        var io = new StubCpuIO(readValue: 0xAB, expectedPort: 0x42);
        var cpu = CreateCpu(mmu, initialState, io);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.Ra = 0xAB;
        expectedState.IncrementPcBy(2);

        Assert.Equal(10, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestInReturnsZeroWhenNoPortBusAttached()
    {
        var mmu = new Mmu();
        mmu.Write(0x00, 0xDB); // IN
        mmu.Write(0x01, 0x01);

        var cpu = CreateCpu(mmu, new CpuState { Pc = 0x00 }); // uses NoOpCpuIO
        cpu.Step();

        Assert.Equal(0x00, cpu.Ra);
    }

    [Fact]
    public void TestOut()
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x55 };
        var mmu = new Mmu();
        mmu.Write(0x00, 0xD3); // OUT
        mmu.Write(0x01, 0x42); // port number

        var io = new CapturingCpuIO();
        var cpu = CreateCpu(mmu, initialState, io);
        var cycles = cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(2);

        Assert.Equal(10, cycles);
        Assert.Equal(0x42, io.LastPort);
        Assert.Equal(0x55, io.LastValue);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestOutDoesNotModifyRegistersOrFlags()
    {
        var initialState = new CpuState { Pc = 0x00, Ra = 0x77, Flags = CpuFlags.C | CpuFlags.Z };
        var mmu = new Mmu();
        mmu.Write(0x00, 0xD3); // OUT
        mmu.Write(0x01, 0x00);

        var cpu = CreateCpu(mmu, initialState, new CapturingCpuIO());
        cpu.Step();

        var expectedState = initialState;
        expectedState.IncrementPcBy(2);

        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }
}

class StubCpuIO(byte readValue, byte expectedPort) : ICpuIO
{
    public byte ReadPort(byte port) => port == expectedPort ? readValue : (byte)0;
    public void WritePort(byte port, byte value) { }
}

class CapturingCpuIO : ICpuIO
{
    public byte LastPort { get; private set; }
    public byte LastValue { get; private set; }

    public byte ReadPort(byte port) => 0;
    public void WritePort(byte port, byte value) { LastPort = port; LastValue = value; }
}
