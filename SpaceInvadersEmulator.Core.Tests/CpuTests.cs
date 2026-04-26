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
        var initialState = new CpuState
        {
            Pc = 0x10
        };

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, 0x00);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        Assert.Equal(4, cycles);
        Assert.Equal(initialState with { Pc = (byte)(initialState.Pc + 1) }, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x40, Reg.B, Reg.B)]
    [InlineData(0x41, Reg.B, Reg.C)]
    [InlineData(0x42, Reg.B, Reg.D)]
    [InlineData(0x43, Reg.B, Reg.E)]
    [InlineData(0x44, Reg.B, Reg.H)]
    [InlineData(0x45, Reg.B, Reg.L)]
    [InlineData(0x47, Reg.B, Reg.A)]
    public void TestMoveRr(byte opcode, Reg dst, Reg src)
    {
        var initialState = new CpuState { Pc = 0x10 };
        initialState.WriteReg(dst, 0x11);
        initialState.WriteReg(src, 0x50);

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState with { Pc = (byte)(initialState.Pc + 1) };
        expectedState.WriteReg(dst, initialState.ReadReg(src));

        Assert.Equal(5, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }

    [Theory]
    [InlineData(0x46, Reg.B)]
    public void TestMoveRm(byte opcode, Reg dst)
    {
        var initialState = new CpuState
        {
            Pc = 0x10,
            Rh = 0x20,
            Rl = 0x30
        };
        initialState.WriteReg(dst, 0x11);
        var address = (ushort)((initialState.Rh << 8) | initialState.Rl);

        var mmu = new Mmu();
        mmu.Write(initialState.Pc, opcode);
        mmu.Write(address, 0x50);

        var cpu = CreateCpu(mmu, initialState);
        var cycles = cpu.Step();

        var expectedState = initialState with
        {
            Pc = (byte)(initialState.Pc + 1),
        };
        expectedState.WriteReg(dst, 0x50);
        
        Assert.Equal(7, cycles);
        Assert.Equal(expectedState, CpuState.FromCpu(cpu));
    }
}
