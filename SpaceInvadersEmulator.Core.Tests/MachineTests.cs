using SpaceInvadersEmulator.Core;

namespace SpaceInvadersEmulator.Core.Tests;

public class MachineTests
{
    private sealed class FakeClock : IClock
    {
        public event Action? Ticked;
        public long Frequency => 1_000_000;

        private long _timestamp;
        public long GetTimestamp() => _timestamp;

        public void Tick(long elapsed = 0)
        {
            _timestamp += elapsed;
            Ticked?.Invoke();
        }
    }

    private static Machine CreateMachine(out FakeClock clock)
    {
        clock = new FakeClock();
        return new Machine(clock);
    }

    // --- Lifecycle ---

    [Fact]
    public void Start_SetsIsRunningTrue()
    {
        var machine = CreateMachine(out _);
        machine.Start();
        Assert.True(machine.IsRunning);
        machine.Stop();
    }

    [Fact]
    public void Stop_SetsIsRunningFalse()
    {
        var machine = CreateMachine(out _);
        machine.Start();
        machine.Stop();
        Assert.False(machine.IsRunning);
    }

    [Fact]
    public void Start_WhenAlreadyRunning_Throws()
    {
        var machine = CreateMachine(out _);
        machine.Start();
        Assert.Throws<InvalidOperationException>(() => machine.Start());
        machine.Stop();
    }

    [Fact]
    public void Stop_WhenNotRunning_DoesNotThrow()
    {
        var machine = CreateMachine(out _);
        machine.Stop();
    }

    // --- Port 4: Shift register data ---

    [Fact]
    public void WritePort4_FirstWrite_LoadsHighByte()
    {
        var machine = CreateMachine(out _);
        machine.WritePort(4, 0xAB);
        machine.WritePort(2, 0); // offset = 0 → read high byte
        Assert.Equal(0xAB, machine.ReadPort(3));
    }

    [Fact]
    public void WritePort4_SubsequentWrite_ShiftsHighByteToLow()
    {
        // After two writes: hi=0xCD, lo=0xAB → value=0xCDAB; offset=0 → 0xCD
        var machine = CreateMachine(out _);
        machine.WritePort(4, 0xAB); // hi=0xAB
        machine.WritePort(4, 0xCD); // lo=0xAB, hi=0xCD
        machine.WritePort(2, 0);
        Assert.Equal(0xCD, machine.ReadPort(3));
    }

    // --- Port 3: Shift register read ---

    // Two writes set up: hi=0xAB, lo=0xCD → 16-bit value=0xABCD
    // ReadPort3 = (0xABCD >> (8 - offset)) & 0xFF
    [Theory]
    [InlineData(0, 0xAB)] // >> 8
    [InlineData(1, 0x57)] // >> 7
    [InlineData(2, 0xAF)] // >> 6
    [InlineData(3, 0x5E)] // >> 5
    [InlineData(4, 0xBC)] // >> 4
    [InlineData(5, 0x79)] // >> 3
    [InlineData(6, 0xF3)] // >> 2
    [InlineData(7, 0xE6)] // >> 1
    public void ReadPort3_WithOffset_ReturnsCorrectWindow(byte offset, byte expected)
    {
        var machine = CreateMachine(out _);
        machine.WritePort(4, 0xCD); // hi=0xCD
        machine.WritePort(4, 0xAB); // lo=0xCD, hi=0xAB → value=0xABCD
        machine.WritePort(2, offset);
        Assert.Equal(expected, machine.ReadPort(3));
    }

    // --- Port 2: Shift offset masking ---

    // Port 2 only uses the low 3 bits (0–7); high bits must be masked out
    [Theory]
    [InlineData(0x08, 0xAB)] // 0x08 & 0x07 = 0 → same result as offset 0
    [InlineData(0xFF, 0xE6)] // 0xFF & 0x07 = 7 → same result as offset 7
    public void WritePort2_HighBitsIgnored(byte portValue, byte expected)
    {
        var machine = CreateMachine(out _);
        machine.WritePort(4, 0xCD);
        machine.WritePort(4, 0xAB); // value=0xABCD
        machine.WritePort(2, portValue);
        Assert.Equal(expected, machine.ReadPort(3));
    }

    // --- Unknown ports ---

    [Theory]
    [InlineData(0)]
    public void ReadPort_UnknownPort_ReturnsZero(byte port)
    {
        var machine = CreateMachine(out _);
        Assert.Equal(0, machine.ReadPort(port));
    }

    // --- Port 1: Coin + player inputs ---

    [Fact]
    public void WriteCoin_WhenPressed_SetsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteCoin(true);
        Assert.Equal(0x01, machine.ReadPort(1) & 0x01);
    }

    [Fact]
    public void WriteCoin_WhenReleased_ClearsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteCoin(true);
        machine.WriteCoin(false);
        Assert.Equal(0, machine.ReadPort(1) & 0x01);
    }

    [Fact]
    public void WriteP1Start_WhenPressed_SetsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteP1Start(true);
        Assert.Equal(0x04, machine.ReadPort(1) & 0x04);
    }

    [Fact]
    public void WriteP1Start_WhenReleased_ClearsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteP1Start(true);
        machine.WriteP1Start(false);
        Assert.Equal(0, machine.ReadPort(1) & 0x04);
    }

    [Fact]
    public void WriteP2Start_WhenPressed_SetsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteP2Start(true);
        Assert.Equal(0x02, machine.ReadPort(1) & 0x02);
    }

    [Fact]
    public void WriteP2Start_WhenReleased_ClearsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteP2Start(true);
        machine.WriteP2Start(false);
        Assert.Equal(0, machine.ReadPort(1) & 0x02);
    }

    [Fact]
    public void WriteP1Fire_WhenPressed_SetsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteP1Fire(true);
        Assert.Equal(0x10, machine.ReadPort(1) & 0x10);
    }

    [Fact]
    public void WriteP1Fire_WhenReleased_ClearsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteP1Fire(true);
        machine.WriteP1Fire(false);
        Assert.Equal(0, machine.ReadPort(1) & 0x10);
    }

    [Fact]
    public void WriteP1Left_WhenPressed_SetsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteP1Left(true);
        Assert.Equal(0x20, machine.ReadPort(1) & 0x20);
    }

    [Fact]
    public void WriteP1Left_WhenReleased_ClearsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteP1Left(true);
        machine.WriteP1Left(false);
        Assert.Equal(0, machine.ReadPort(1) & 0x20);
    }

    [Fact]
    public void WriteP1Right_WhenPressed_SetsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteP1Right(true);
        Assert.Equal(0x40, machine.ReadPort(1) & 0x40);
    }

    [Fact]
    public void WriteP1Right_WhenReleased_ClearsBitInPort1()
    {
        var machine = CreateMachine(out _);
        machine.WriteP1Right(true);
        machine.WriteP1Right(false);
        Assert.Equal(0, machine.ReadPort(1) & 0x40);
    }

    // --- Port 2: Player 2 inputs ---

    [Fact]
    public void WriteP2Fire_WhenPressed_SetsBitInPort2()
    {
        var machine = CreateMachine(out _);
        machine.WriteP2Fire(true);
        Assert.Equal(0x10, machine.ReadPort(2) & 0x10);
    }

    [Fact]
    public void WriteP2Fire_WhenReleased_ClearsBitInPort2()
    {
        var machine = CreateMachine(out _);
        machine.WriteP2Fire(true);
        machine.WriteP2Fire(false);
        Assert.Equal(0, machine.ReadPort(2) & 0x10);
    }

    [Fact]
    public void WriteP2Left_WhenPressed_SetsBitInPort2()
    {
        var machine = CreateMachine(out _);
        machine.WriteP2Left(true);
        Assert.Equal(0x20, machine.ReadPort(2) & 0x20);
    }

    [Fact]
    public void WriteP2Left_WhenReleased_ClearsBitInPort2()
    {
        var machine = CreateMachine(out _);
        machine.WriteP2Left(true);
        machine.WriteP2Left(false);
        Assert.Equal(0, machine.ReadPort(2) & 0x20);
    }

    [Fact]
    public void WriteP2Right_WhenPressed_SetsBitInPort2()
    {
        var machine = CreateMachine(out _);
        machine.WriteP2Right(true);
        Assert.Equal(0x40, machine.ReadPort(2) & 0x40);
    }

    [Fact]
    public void WriteP2Right_WhenReleased_ClearsBitInPort2()
    {
        var machine = CreateMachine(out _);
        machine.WriteP2Right(true);
        machine.WriteP2Right(false);
        Assert.Equal(0, machine.ReadPort(2) & 0x40);
    }
}
