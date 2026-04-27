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
    [InlineData(1)]
    [InlineData(2)]
    public void ReadPort_UnknownPort_ReturnsZero(byte port)
    {
        var machine = CreateMachine(out _);
        Assert.Equal(0, machine.ReadPort(port));
    }
}
