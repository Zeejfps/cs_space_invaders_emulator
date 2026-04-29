using System.Runtime.CompilerServices;
using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core;

public sealed class Machine
{
    private const int CpuFrequency = 2_000_000;
    private const double CyclesPerHalfFrame = CpuFrequency / 60.0 / 2.0;
    private const Port2 ShipsMask = Port2.ShipsBit0 | Port2.ShipsBit1;

    public ReadOnlyMemory<byte> VRam => _mmu.VRam;

    public bool IsPoweredOn { get; private set; }

    public ShipCount Ships
    {
        get => (ShipCount)((int)_ioBus.ReadBit(ShipsMask) + 3);
        set => _ioBus.WriteBit(ShipsMask, (Port2)((int)value - 3));
    }

    public BonusLifeThreshold BonusLife
    {
        get => _ioBus.ReadBit(Port2.BonusAt1000) != 0
            ? BonusLifeThreshold.At1000
            : BonusLifeThreshold.At1500;
        set => _ioBus.WriteBit(Port2.BonusAt1000,
            value == BonusLifeThreshold.At1000 ? Port2.BonusAt1000 : 0);
    }

    private readonly IClock _clock;
    private readonly Mmu _mmu;
    private readonly Cpu _cpu;
    private readonly IOBus _ioBus;

    private long _lastTimestamp;

    private double _cycleCount;
    private readonly double _cyclesPerTick;

    private double _frameCycles;
    private byte _nextInterrupt = 0xCF;

    public Machine(IClock clock, IAudio? audio = null)
    {
        _mmu = new Mmu();
        _ioBus = new IOBus(audio);
        _cpu = new Cpu(_mmu, _ioBus)
        {
            InterruptEnabled = true
        };
        _clock = clock;
        _cyclesPerTick = CpuFrequency / (double)_clock.Frequency;
    }

    public void LoadRom(ReadOnlySpan<byte> rom)
    {
        // Full reset so a second LoadRom (game switch) doesn't inherit RAM,
        // CPU registers, shift register, or sound-edge state from the previous run.
        // _port2 (Ships / BonusLife dipswitches) is preserved — callers set those
        // explicitly per-game after LoadRom.
        if (IsPoweredOn) PowerOff();
        _mmu.Reset();
        _cpu.Reset();
        _ioBus.Reset();
        
        _cycleCount = 0;
        _frameCycles = 0;
        _nextInterrupt = 0xCF;

        _mmu.LoadRom(rom);
        _cpu.Pc = _mmu.RomStartAddress;
    }

    public void PowerOn()
    {
        if (IsPoweredOn)
            throw new InvalidOperationException("Machine is already running");

        _clock.Ticked += Clock_OnTick;
        _lastTimestamp = _clock.GetTimestamp();
        IsPoweredOn = true;
    }

    public void PowerOff()
    {
        if (!IsPoweredOn)
            return;

        IsPoweredOn = false;
        _clock.Ticked -= Clock_OnTick;
    }

    public void WriteCoin(bool pressed) => _ioBus.WriteInput(Port1.InsertCoin, pressed);
    public void WriteP1Start(bool pressed) => _ioBus.WriteInput(Port1.Player1Start, pressed);
    public void WriteP2Start(bool pressed) => _ioBus.WriteInput(Port1.Player2Start, pressed);
    public void WriteP1Fire(bool pressed) => _ioBus.WriteInput(Port1.Player1Fire, pressed);
    public void WriteP1Left(bool pressed) => _ioBus.WriteInput(Port1.Player1Left, pressed);
    public void WriteP1Right(bool pressed) => _ioBus.WriteInput(Port1.Player1Right, pressed);
    public void WriteP2Fire(bool pressed) => _ioBus.WriteInput(Port2.Player2Fire, pressed);
    public void WriteP2Left(bool pressed) => _ioBus.WriteInput(Port2.Player2Left, pressed);
    public void WriteP2Right(bool pressed) => _ioBus.WriteInput(Port2.Player2Right, pressed);

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void Clock_OnTick()
    {
        var timestamp = _clock.GetTimestamp();
        var elapsedTime = timestamp - _lastTimestamp;
        _lastTimestamp = timestamp;

        _cycleCount += elapsedTime * _cyclesPerTick;
        while (_cycleCount > 0)
        {
            var cycles = _cpu.Step();
            _cycleCount -= cycles;
            _frameCycles += cycles;

            if (_frameCycles >= CyclesPerHalfFrame)
            {
                _frameCycles -= CyclesPerHalfFrame;
                _cpu.Interrupt(_nextInterrupt);
                _nextInterrupt = _nextInterrupt == 0xCF ? (byte)0xD7 : (byte)0xCF;
            }
        }
    }
}
