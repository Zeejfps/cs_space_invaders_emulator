using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core.Tests;

public static class CpuExtensions
{
    public static Cpu WriteState(this Cpu cpu, CpuState state)
    {
        cpu.Flags = state.Flags;
        cpu.Pc = state.Pc;
        cpu.Sp = state.Sp;
        cpu.Ra = state.Ra;
        cpu.Rb = state.Rb;
        cpu.Rc = state.Rc;
        cpu.Rd = state.Rd;
        cpu.Re = state.Re;
        cpu.Rh = state.Rh;
        cpu.Rl = state.Rl;
        return cpu;
    }

    public static CpuState ReadState(this Cpu cpu)
    {
        return CpuState.FromCpu(cpu);
    }
}

class NoOpCpuIO : ICpuIO
{
    public byte ReadPort(byte port) => 0;
    public void WritePort(byte port, byte value) { }
}

public class FakeMmu : IMmu
{
    private readonly byte[] _ram = new byte[64 * 1024];

    public void Write(ushort address, byte value) => _ram[address] = value;

    public void WriteWord(ushort address, ushort value)
    {
        _ram[address] = (byte)(value & 0xFF);
        _ram[(ushort)(address + 1)] = (byte)(value >> 8);
    }

    public byte Read(ushort address) => _ram[address];

    public ushort ReadWord(ushort address) =>
        (ushort)((_ram[(ushort)(address + 1)] << 8) | _ram[address]);
}
