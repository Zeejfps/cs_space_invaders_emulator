using System.Runtime.CompilerServices;
using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core;

public sealed class Machine
{
    private const int CpuFrequency = 2_000_000;
    private const double CyclesPerHalfFrame = CpuFrequency / 60.0 / 2.0;
    private const Port2 ShipsMask = Port2.ShipsBit0 | Port2.ShipsBit1;

    public ReadOnlyMemory<byte> VRam => _mmu.VRam;

    public bool IsRunning { get; private set; }

    public ShipCount Ships
    {
        get => (ShipCount)((int)(_ioBus.Port2 & ShipsMask) + 3);
        set => _ioBus.Port2 = (_ioBus.Port2 & ~ShipsMask) | (Port2)((int)value - 3);
    }

    public BonusLifeThreshold BonusLife
    {
        get => _ioBus.Port2.HasFlag(Port2.BonusAt1000) ? BonusLifeThreshold.At1000 : BonusLifeThreshold.At1500;
        set
        {
            if (value == BonusLifeThreshold.At1000) _ioBus.Port2 |= Port2.BonusAt1000;
            else _ioBus.Port2 &= ~Port2.BonusAt1000;
        }
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
        if (IsRunning) Stop();
        _mmu.Reset();
        _cpu.Reset();
        _ioBus.Reset();
        
        _cycleCount = 0;
        _frameCycles = 0;
        _nextInterrupt = 0xCF;

        _mmu.LoadRom(rom);
        _cpu.Pc = _mmu.RomStartAddress;
    }

    public void Start()
    {
        if (IsRunning)
            throw new InvalidOperationException("Machine is already running");

        _clock.Ticked += Clock_OnTick;
        _lastTimestamp = _clock.GetTimestamp();
        IsRunning = true;
    }

    public void Stop()
    {
        if (!IsRunning)
            return;

        IsRunning = false;
        _clock.Ticked -= Clock_OnTick;
    }

    public void WriteCoin(bool pressed) => _ioBus.WritePort1Input(Port1.InsertCoin, pressed);
    public void WriteP1Start(bool pressed) => _ioBus.WritePort1Input(Port1.Player1Start, pressed);
    public void WriteP2Start(bool pressed) => _ioBus.WritePort1Input(Port1.Player2Start, pressed);
    public void WriteP1Fire(bool pressed) => _ioBus.WritePort1Input(Port1.Player1Fire, pressed);
    public void WriteP1Left(bool pressed) => _ioBus.WritePort1Input(Port1.Player1Left, pressed);
    public void WriteP1Right(bool pressed) => _ioBus.WritePort1Input(Port1.Player1Right, pressed);
    public void WriteP2Fire(bool pressed) => _ioBus.WritePort2Input(Port2.Player2Fire, pressed);
    public void WriteP2Left(bool pressed) => _ioBus.WritePort2Input(Port2.Player2Left, pressed);
    public void WriteP2Right(bool pressed) => _ioBus.WritePort2Input(Port2.Player2Right, pressed);

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
