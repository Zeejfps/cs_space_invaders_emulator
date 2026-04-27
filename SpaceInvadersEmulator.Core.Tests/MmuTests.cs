namespace SpaceInvadersEmulator.Core.Tests;

public class MmuTests
{
    [Fact]
    public void TestWriteRead()
    {
        var mmu = new Mmu();
        mmu.Write(0x2010, 0x55);

        var value = mmu.Read(0x2010);
        Assert.Equal(0x55, value);
    }
}