namespace SpaceInvadersEmulator.Core.Tests;

public class CpuTests
{
    [Fact]
    public void TestNoOp()
    {
        var startingFlags = CpuFlags.None;
        byte startingPc = 0x0;
        byte startingSp = 0x0;
        byte startingRa = 0x0;
        byte startingRb = 0x0;
        byte startingRc = 0x0;
        byte startingRd = 0x0;
        byte startingRe = 0x0;
        byte startingRh = 0x0;
        byte startingRl = 0x0;
        var noOpInsCycles = 4;
        
        var mmu = new Mmu();
        mmu.Write(0x0, 0x00);
        
        var cpu = new Cpu(mmu)
        {
            Flags = startingFlags,
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

    [Fact]
    public void TestMoveBB()
    {
        var startingFlags = CpuFlags.None;
        byte startingPc = 0x0;
        byte startingSp = 0x0;
        byte startingRa = 0x0;
        byte startingRb = 0x0;
        byte startingRc = 0x0;
        byte startingRd = 0x0;
        byte startingRe = 0x0;
        byte startingRh = 0x0;
        byte startingRl = 0x0;
        var noOpInsCycles = 4;
        
        var mmu = new Mmu();
        mmu.Write(0x0, 0x40);
        
        var cpu = new Cpu(mmu)
        {
            Flags = startingFlags,
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
    
    [Fact]
    public void TestMoveBC()
    {
        var startingFlags = CpuFlags.None;
        byte startingPc = 0x0;
        byte startingSp = 0x0;
        byte startingRa = 0x0;
        byte startingRb = 0x0;
        byte startingRc = 0x0;
        byte startingRd = 0x0;
        byte startingRe = 0x0;
        byte startingRh = 0x0;
        byte startingRl = 0x0;
        var noOpInsCycles = 4;
        
        var mmu = new Mmu();
        mmu.Write(0x0, 0x40);
        
        var cpu = new Cpu(mmu)
        {
            Flags = startingFlags,
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