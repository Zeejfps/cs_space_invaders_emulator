namespace SpaceInvadersEmulator.Core.Tests;

public class CpuTests
{
    [Fact]
    public void Test1()
    {
        var startingFlags = 0x0;
        var startingPc = 0x0;
        var startingSp = 0x0;
        var startingRa = 0x0;
        var startingRb = 0x0;
        var startingRc = 0x0;
        var startingRd = 0x0;
        var startingRe = 0x0;
        var startingRh = 0x0;
        var startingRl = 0x0;
        var noOpInsCycles = 4;
        
        var mmu = new Mmu();
        mmu.Write(0x0, 0x00);
        
        var cpu = new Cpu(mmu)
        {
            Flags = 0x0,
            Pc = startingPc,
            Sp = startingSp,
            Ra = startingRa,
            Rb = startingRb,
            Rc = startingRc,
            Rd = startingRd,
            Re = startingRe,
            Rh = startingRh,
            Rl = startingRl
        };

        var cycles = cpu.Step();

        Assert.Equal(noOpInsCycles, cycles);
        Assert.Equal(startingPc + 1, cpu.Pc);
        Assert.Equal(startingFlags, cpu.Flags);
        Assert.Equal(startingSp, cpu.Sp);
        Assert.Equal(startingRa, cpu.Ra);
        Assert.Equal(startingRb, cpu.Rb);
        Assert.Equal(startingRc, cpu.Rc);
        Assert.Equal(startingRd, cpu.Rd);
        Assert.Equal(startingRe, cpu.Re);
        Assert.Equal(startingRh, cpu.Rh);
        Assert.Equal(startingRl, cpu.Rl);
    }
}