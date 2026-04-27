using System.Runtime.CompilerServices;
using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core;

public sealed class Machine : ICpuIO
{
    private const int CpuFrequency = 2_000_000;
    private const double CyclesPerHalfFrame = CpuFrequency / 60.0 / 2.0;
    private const Port2 ShipsMask = Port2.ShipsBit0 | Port2.ShipsBit1;

    public ReadOnlyMemory<byte> VRam => _mmu.VRam;

    public bool IsRunning { get; private set; }

    public ShipCount Ships
    {
        get => (ShipCount)((int)(_port2 & ShipsMask) + 3);
        set => _port2 = (_port2 & ~ShipsMask) | (Port2)((int)value - 3);
    }

    public BonusLifeThreshold BonusLife
    {
        get => _port2.HasFlag(Port2.BonusAt1000) ? BonusLifeThreshold.At1000 : BonusLifeThreshold.At1500;
        set
        {
            if (value == BonusLifeThreshold.At1000) _port2 |= Port2.BonusAt1000;
            else _port2 &= ~Port2.BonusAt1000;
        }
    }

    private readonly IClock _clock;
    private readonly IAudio? _audio;
    private readonly Mmu _mmu;
    private readonly Cpu _cpu;

    private byte _shiftRegHi;
    private byte _shiftRegLo;
    private byte _shiftRegOffset;
    private Port3 _prevPort3Value;
    private Port5 _prevPort5Value;

    private long _lastTimestamp;

    private double _cycleCount;
    private readonly double _cyclesPerTick;

    private double _frameCycles;
    private byte _nextInterrupt = 0xCF;

    private Port1 _port1;
    private Port2 _port2;

    public Machine(IClock clock, IAudio? audio = null)
    {
        _mmu = new Mmu();
        _cpu = new Cpu(_mmu, this)
        {
            InterruptEnabled = true
        };
        _clock = clock;
        _audio = audio;
        _cyclesPerTick = CpuFrequency / (double)_clock.Frequency;
    }

    public void LoadRom(ReadOnlySpan<byte> rom)
    {
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

    public void WriteCoin(bool pressed) => WritePort1Input(Port1.InsertCoin, pressed);
    public void WriteP1Start(bool pressed) => WritePort1Input(Port1.Player1Start, pressed);
    public void WriteP2Start(bool pressed) => WritePort1Input(Port1.Player2Start, pressed);
    public void WriteP1Fire(bool pressed) => WritePort1Input(Port1.Player1Fire, pressed);
    public void WriteP1Left(bool pressed) => WritePort1Input(Port1.Player1Left, pressed);
    public void WriteP1Right(bool pressed) => WritePort1Input(Port1.Player1Right, pressed);
    public void WriteP2Fire(bool pressed) => WritePort2Input(Port2.Player2Fire, pressed);
    public void WriteP2Left(bool pressed) => WritePort2Input(Port2.Player2Left, pressed);
    public void WriteP2Right(bool pressed) => WritePort2Input(Port2.Player2Right, pressed);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WritePort1Input(Port1 flag, bool pressed)
    {
        if (pressed) _port1 |= flag;
        else _port1 &= ~flag;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WritePort2Input(Port2 flag, bool pressed)
    {
        if (pressed) _port2 |= flag;
        else _port2 &= ~flag;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void WritePort(byte port, byte value)
    {
        switch (port)
        {
            case 2:
                WritePort2(value);
                break;
            case 3:
                WritePort3(value);
                break;
            case 4:
                WritePort4(value);
                break;
            case 5:
                WritePort5(value);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte ReadPort(byte port)
    {
        return port switch
        {
            1 => (byte)_port1,
            2 => (byte)_port2,
            3 => ReadPort3(),
            _ => 0
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WritePort4(byte value)
    {
        _shiftRegLo = _shiftRegHi;
        _shiftRegHi = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WritePort2(byte value)
    {
        _shiftRegOffset = (byte)(value & 0x07);
    }

    private void WritePort3(byte raw)
    {
        if (_audio == null) return;
        var value = (Port3)raw;
        var rising = ~_prevPort3Value & value;
        var changed = _prevPort3Value ^ value;

        if ((changed & Port3.UfoLoop) != Port3.None)
            _audio.UfoLoop(value.HasFlag(Port3.UfoLoop));
        
        if ((rising & Port3.Shot) != Port3.None)
            _audio.PlayShot();
        
        if ((rising & Port3.PlayerDie) != Port3.None)
            _audio.PlayPlayerDied();
        
        if ((rising & Port3.InvaderDie) != Port3.None)
            _audio.PlayInvaderDied();
        
        if ((rising & Port3.ExtendedPlay) != Port3.None)
            _audio.ExtendedPlay();
        
        _prevPort3Value = value;
    }

    private void WritePort5(byte raw)
    {
        if (_audio == null) return;
        var value = (Port5)raw;
        var rising = ~_prevPort5Value & value;

        if ((rising & Port5.FleetMove1) != Port5.None)
            _audio.PlayFleetMoved(1);
        
        if ((rising & Port5.FleetMove2) != Port5.None)
            _audio.PlayFleetMoved(2);
        
        if ((rising & Port5.FleetMove3) != Port5.None)
            _audio.PlayFleetMoved(3);
        
        if ((rising & Port5.FleetMove4) != Port5.None)
            _audio.PlayFleetMoved(4);
        
        if ((rising & Port5.UfoHit) != Port5.None)
            _audio.PlayUfoHit();
        
        _prevPort5Value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte ReadPort3()
    {
        var value = (_shiftRegHi << 8) | _shiftRegLo;
        return (byte)((value >> (8 - _shiftRegOffset)) & 0xFF);
    }
}
