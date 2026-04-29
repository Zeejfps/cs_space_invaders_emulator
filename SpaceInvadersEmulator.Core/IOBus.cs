using System.Runtime.CompilerServices;
using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core;

internal sealed class IOBus : IIOBus
{
    public Port2 Port2 { get; set; }

    private Port1 _port1;
    private Port3 _prevPort3Value;
    private Port5 _prevPort5Value;
    private byte _shiftRegHi;
    private byte _shiftRegLo;
    private byte _shiftRegOffset;
    
    private readonly IAudio? _audio;

    public IOBus(IAudio? audio)
    {
        _audio = audio;
    }

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

    public byte ReadPort(byte port)
    {
        return port switch
        {
            1 => (byte)_port1,
            2 => (byte)Port2,
            3 => ReadPort3(),
            _ => 0
        };    }
    
    public void WriteInput(Port1 flag, bool pressed)
    {
        if (pressed) _port1 |= flag;
        else _port1 &= ~flag;
    }

    public void WriteInput(Port2 flag, bool pressed)
    {
        if (pressed) Port2 |= flag;
        else Port2 &= ~flag;
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
        if (_audio == null)
            return;
        
        var value = (Port3)raw;
        var rising = ~_prevPort3Value & value;
        var changed = _prevPort3Value ^ value;

        if ((changed & Port3.UfoLoop) != Port3.None)
            _audio.SetUfoLoopActive(value.HasFlag(Port3.UfoLoop));
        
        if ((rising & Port3.Shot) != Port3.None)
            _audio.PlayShot();
        
        if ((rising & Port3.PlayerDie) != Port3.None)
            _audio.PlayPlayerDied();
        
        if ((rising & Port3.InvaderDie) != Port3.None)
            _audio.PlayInvaderDied();
        
        if ((rising & Port3.ExtendedPlay) != Port3.None)
            _audio.PlayExtraLifeGained();
        
        _prevPort3Value = value;
    }

    private void WritePort5(byte raw)
    {
        if (_audio == null) 
            return;
        
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

    public void Reset()
    {
        _shiftRegHi = 0;
        _shiftRegLo = 0;
        _shiftRegOffset = 0;
        _prevPort3Value = 0;
        _prevPort5Value = 0;
        _port1 = 0;
    }
}