using System.Runtime.CompilerServices;
using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core;

public sealed class Machine : ICpuIO
{
    public bool IsRunning { get; private set; }
    
    private readonly IClock _clock;
    private readonly Mmu _mmu;
    private readonly Cpu _cpu;

    private byte _shiftRegHi;
    private byte _shiftRegLo;
    private byte _shiftRegOffset;

    private long _lastTimestamp;
    private int _cycleCount;

    public Machine(IClock clock)
    {
        _clock = clock;
    }
    
    public void LoadRom(ReadOnlySpan<byte> rom)
    {
        _mmu.Write(0x0, rom);
        _cpu.Pc = 0x0;
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

    private void OnTick()
    {
        var timestamp = _clock.GetTimestamp();
        var elapsedTime = timestamp - _lastTimestamp;
        _lastTimestamp = timestamp;
        
        _cycleCount += _cpu.Step();
    }

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
        _shiftRegOffset = (byte)(value * 0x07);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte ReadPort3()
    {
        var value = (_shiftRegHi << 8) | _shiftRegLo;
        return (byte)((value >> (8 - _shiftRegOffset)) & 0xFF);
    }
}