using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core;

public sealed class Machine
{
    private readonly Mmu _mmu;
    private readonly Cpu _cpu;

    private int _cycleCount;
    
    public void Start()
    {
        
    }
    
    public void Stop()
    {
        
    }

    public void LoadRom(ReadOnlySpan<byte> rom)
    {
        _mmu.Write(0x0, rom);
        _cpu.Pc = 0x0;
    }

    private void OnTick()
    {
        _cycleCount += _cpu.Step();
    }
}