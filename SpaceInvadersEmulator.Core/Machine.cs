using System.Runtime.CompilerServices;
using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core;

public sealed class Machine : ICpuIO
{
    private const int CpuFrequency = 2_000_000;
    
    public bool IsRunning { get; private set; }
    
    private readonly IClock _clock;
    private readonly Mmu _mmu;
    private readonly Cpu _cpu;

    private byte _shiftRegHi;
    private byte _shiftRegLo;
    private byte _shiftRegOffset;

    private long _lastTimestamp;

    private double _cycleCount;
    private readonly double _cyclesPerTick;

    private double _frameCycles;
    private byte _nextInterrupt = 0xCF;

    public Machine(IClock clock)
    {
        _mmu = new Mmu();
        _cpu = new Cpu(_mmu, this)
        {
            InterruptEnabled = true
        };
        _clock = clock;
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
        
        _clock.Ticked += OnTick;
        _lastTimestamp = _clock.GetTimestamp();
        IsRunning = true;
    }
    
    public void Stop()
    {
        if (!IsRunning)
            return;
        
        IsRunning = false;
        _clock.Ticked -= OnTick;
    }

    private const double CyclesPerHalfFrame = CpuFrequency / 60.0 / 2.0;

    private void OnTick()
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
            case 4:
                WritePort4(value);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte ReadPort(byte port)
    {
        return port switch
        {
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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte ReadPort3()
    {
        var value = (_shiftRegHi << 8) | _shiftRegLo;
        return (byte)((value >> (8 - _shiftRegOffset)) & 0xFF);
    }
}