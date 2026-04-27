namespace SpaceInvadersEmulator.Core.Tests;

static class CpuTestHelper
{
    public static Cpu CreateCpu(Mmu mmu, CpuState state) => new(mmu)
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
