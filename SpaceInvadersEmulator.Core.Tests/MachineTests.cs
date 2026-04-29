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
    public void PowerOn_SetsIsRunningTrue()
    {
        var machine = CreateMachine(out _);
        machine.PowerOn();
        Assert.True(machine.IsPoweredOn);
        machine.PowerOff();
    }

    [Fact]
    public void PowerOff_SetsIsRunningFalse()
    {
        var machine = CreateMachine(out _);
        machine.PowerOn();
        machine.PowerOff();
        Assert.False(machine.IsPoweredOn);
    }

    [Fact]
    public void PowerOn_WhenAlreadyRunning_Throws()
    {
        var machine = CreateMachine(out _);
        machine.PowerOn();
        Assert.Throws<InvalidOperationException>(() => machine.PowerOn());
        machine.PowerOff();
    }

    [Fact]
    public void PowerOff_WhenNotRunning_DoesNotThrow()
    {
        var machine = CreateMachine(out _);
        machine.PowerOff();
    }
}
