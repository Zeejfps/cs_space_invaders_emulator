using SpaceInvadersEmulator.Core.Intel8080;

namespace SpaceInvadersEmulator.Core.Tests;

static class CpuTestHelper
{
    public static Cpu CreateCpu(Mmu mmu, CpuState state, ICpuIO? io = null) => new(mmu, io ?? new NoOpCpuIO())
    {
        Flags = state.Flags,
        Pc = state.Pc,
        Sp = state.Sp,
        Ra = state.Ra,
        Rb = state.Rb,
        Rc = state.Rc,
        Rd = state.Rd,
        Re = state.Re,
        Rh = state.Rh,
        Rl = state.Rl
    };
}

class NoOpCpuIO : ICpuIO
{
    public byte ReadPort(byte port) => 0;
    public void WritePort(byte port, byte value) { }
}
