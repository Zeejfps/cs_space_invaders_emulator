namespace SpaceInvadersEmulator.Core.Tests;

public class CpuTests
{
    private static Cpu CreateCpu(Mmu mmu, CpuState state) => new(mmu)
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

[Fact]
    public void TestNoOp()
    {
        var initialState = new CpuState();
        var mmu = new Mmu();
        mmu.Write(0x0, 0x00);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        Assert.Equal(4, cycles);
        Assert.Equal(initialState with { Pc = (byte)(initialState.Pc + 1) }, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestMoveBb()
    {
        var initialState = new CpuState();
        
        var mmu = new Mmu();
        mmu.Write(0x0, 0x40);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState with
        {
            Pc = (byte)(initialState.Pc + 1)
        };
        
        Assert.Equal(4, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Fact]
    public void TestMoveBc()
    {
        var initialState = new CpuState
        {
            Rc = 0x50
        };
        
        var mmu = new Mmu();
        mmu.Write(0x0, 0x41);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState with
        {
            Pc = (byte)(initialState.Pc + 1),
            Rb = initialState.Rc
        };
        
        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }
}
