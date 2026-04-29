using SpaceInvadersEmulator.Core;

namespace SpaceInvadersEmulator.Core.Tests;

public class IOBusTests
{
    private sealed class FakeAudio : IAudio
    {
        public int UfoLoopCallCount;
        public bool? LastUfoLoopActive;
        public int ShotCallCount;
        public int PlayerDieCallCount;
        public int InvaderDieCallCount;
        public int ExtendedPlayCallCount;
        public List<int> FleetMoveSteps = [];
        public int UfoHitCallCount;

        public void SetUfoLoopActive(bool active) { UfoLoopCallCount++; LastUfoLoopActive = active; }
        public void PlayShot() => ShotCallCount++;
        public void PlayPlayerDied() => PlayerDieCallCount++;
        public void PlayInvaderDied() => InvaderDieCallCount++;
        public void PlayExtraLifeGained() => ExtendedPlayCallCount++;
        public void PlayFleetMoved(int step) => FleetMoveSteps.Add(step);
        public void PlayUfoHit() => UfoHitCallCount++;
    }

    private static IOBus CreateBus() => new IOBus(audio: null);
    private static IOBus CreateBusWithAudio(out FakeAudio audio)
    {
        audio = new FakeAudio();
        return new IOBus(audio);
    }

    // --- Port 4: Shift register data ---

    [Fact]
    public void WritePort4_FirstWrite_LoadsHighByte()
    {
        var bus = CreateBus();
        bus.WritePort(4, 0xAB);
        bus.WritePort(2, 0); // offset = 0 → read high byte
        Assert.Equal(0xAB, bus.ReadPort(3));
    }

    [Fact]
    public void WritePort4_SubsequentWrite_ShiftsHighByteToLow()
    {
        // After two writes: hi=0xCD, lo=0xAB → value=0xCDAB; offset=0 → 0xCD
        var bus = CreateBus();
        bus.WritePort(4, 0xAB); // hi=0xAB
        bus.WritePort(4, 0xCD); // lo=0xAB, hi=0xCD
        bus.WritePort(2, 0);
        Assert.Equal(0xCD, bus.ReadPort(3));
    }

    // --- Port 3 (read): Shift register read ---

    // Two writes set up: hi=0xAB, lo=0xCD → 16-bit value=0xABCD
    // ReadPort3 = (0xABCD >> (8 - offset)) & 0xFF
    [Theory]
    [InlineData(0, 0xAB)] // >> 8
    [InlineData(1, 0x57)] // >> 7
    [InlineData(2, 0xAF)] // >> 6
    [InlineData(3, 0x5E)] // >> 5
    [InlineData(4, 0xBC)] // >> 4
    [InlineData(5, 0x79)] // >> 3
    [InlineData(6, 0xF3)] // >> 2
    [InlineData(7, 0xE6)] // >> 1
    public void ReadPort3_WithOffset_ReturnsCorrectWindow(byte offset, byte expected)
    {
        var bus = CreateBus();
        bus.WritePort(4, 0xCD); // hi=0xCD
        bus.WritePort(4, 0xAB); // lo=0xCD, hi=0xAB → value=0xABCD
        bus.WritePort(2, offset);
        Assert.Equal(expected, bus.ReadPort(3));
    }

    // --- Port 2 (write): Shift offset masking ---

    // Port 2 only uses the low 3 bits (0–7); high bits must be masked out
    [Theory]
    [InlineData(0x08, 0xAB)] // 0x08 & 0x07 = 0 → same result as offset 0
    [InlineData(0xFF, 0xE6)] // 0xFF & 0x07 = 7 → same result as offset 7
    public void WritePort2_HighBitsIgnored(byte portValue, byte expected)
    {
        var bus = CreateBus();
        bus.WritePort(4, 0xCD);
        bus.WritePort(4, 0xAB); // value=0xABCD
        bus.WritePort(2, portValue);
        Assert.Equal(expected, bus.ReadPort(3));
    }

    // --- Unknown ports ---

    [Theory]
    [InlineData(0)]
    public void ReadPort_UnknownPort_ReturnsZero(byte port)
    {
        var bus = CreateBus();
        Assert.Equal(0, bus.ReadPort(port));
    }

    // --- Port 1: Coin + player inputs ---

    [Fact]
    public void WriteInput_CoinPressed_SetsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.InsertCoin, true);
        Assert.Equal(0x01, bus.ReadPort(1) & 0x01);
    }

    [Fact]
    public void WriteInput_CoinReleased_ClearsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.InsertCoin, true);
        bus.WriteInput(Port1.InsertCoin, false);
        Assert.Equal(0, bus.ReadPort(1) & 0x01);
    }

    [Fact]
    public void WriteInput_P1StartPressed_SetsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.Player1Start, true);
        Assert.Equal(0x04, bus.ReadPort(1) & 0x04);
    }

    [Fact]
    public void WriteInput_P1StartReleased_ClearsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.Player1Start, true);
        bus.WriteInput(Port1.Player1Start, false);
        Assert.Equal(0, bus.ReadPort(1) & 0x04);
    }

    [Fact]
    public void WriteInput_P2StartPressed_SetsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.Player2Start, true);
        Assert.Equal(0x02, bus.ReadPort(1) & 0x02);
    }

    [Fact]
    public void WriteInput_P2StartReleased_ClearsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.Player2Start, true);
        bus.WriteInput(Port1.Player2Start, false);
        Assert.Equal(0, bus.ReadPort(1) & 0x02);
    }

    [Fact]
    public void WriteInput_P1FirePressed_SetsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.Player1Fire, true);
        Assert.Equal(0x10, bus.ReadPort(1) & 0x10);
    }

    [Fact]
    public void WriteInput_P1FireReleased_ClearsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.Player1Fire, true);
        bus.WriteInput(Port1.Player1Fire, false);
        Assert.Equal(0, bus.ReadPort(1) & 0x10);
    }

    [Fact]
    public void WriteInput_P1LeftPressed_SetsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.Player1Left, true);
        Assert.Equal(0x20, bus.ReadPort(1) & 0x20);
    }

    [Fact]
    public void WriteInput_P1LeftReleased_ClearsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.Player1Left, true);
        bus.WriteInput(Port1.Player1Left, false);
        Assert.Equal(0, bus.ReadPort(1) & 0x20);
    }

    [Fact]
    public void WriteInput_P1RightPressed_SetsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.Player1Right, true);
        Assert.Equal(0x40, bus.ReadPort(1) & 0x40);
    }

    [Fact]
    public void WriteInput_P1RightReleased_ClearsBitInPort1()
    {
        var bus = CreateBus();
        bus.WriteInput(Port1.Player1Right, true);
        bus.WriteInput(Port1.Player1Right, false);
        Assert.Equal(0, bus.ReadPort(1) & 0x40);
    }

    // --- Port 2: Player 2 inputs ---

    [Fact]
    public void WriteInput_P2FirePressed_SetsBitInPort2()
    {
        var bus = CreateBus();
        bus.WriteInput(Port2.Player2Fire, true);
        Assert.Equal(0x10, bus.ReadPort(2) & 0x10);
    }

    [Fact]
    public void WriteInput_P2FireReleased_ClearsBitInPort2()
    {
        var bus = CreateBus();
        bus.WriteInput(Port2.Player2Fire, true);
        bus.WriteInput(Port2.Player2Fire, false);
        Assert.Equal(0, bus.ReadPort(2) & 0x10);
    }

    [Fact]
    public void WriteInput_P2LeftPressed_SetsBitInPort2()
    {
        var bus = CreateBus();
        bus.WriteInput(Port2.Player2Left, true);
        Assert.Equal(0x20, bus.ReadPort(2) & 0x20);
    }

    [Fact]
    public void WriteInput_P2LeftReleased_ClearsBitInPort2()
    {
        var bus = CreateBus();
        bus.WriteInput(Port2.Player2Left, true);
        bus.WriteInput(Port2.Player2Left, false);
        Assert.Equal(0, bus.ReadPort(2) & 0x20);
    }

    [Fact]
    public void WriteInput_P2RightPressed_SetsBitInPort2()
    {
        var bus = CreateBus();
        bus.WriteInput(Port2.Player2Right, true);
        Assert.Equal(0x40, bus.ReadPort(2) & 0x40);
    }

    [Fact]
    public void WriteInput_P2RightReleased_ClearsBitInPort2()
    {
        var bus = CreateBus();
        bus.WriteInput(Port2.Player2Right, true);
        bus.WriteInput(Port2.Player2Right, false);
        Assert.Equal(0, bus.ReadPort(2) & 0x40);
    }

    // --- ReadBit / WriteBit ---

    [Fact]
    public void WriteBit_Port2_SetsMaskedBitsAndPreservesOthers()
    {
        var bus = CreateBus();
        bus.WriteInput(Port2.Player2Fire, true); // unrelated bit set
        bus.WriteBit(Port2.BonusAt1000, Port2.BonusAt1000);
        Assert.Equal(Port2.BonusAt1000, bus.ReadBit(Port2.BonusAt1000));
        Assert.Equal(Port2.Player2Fire, bus.ReadBit(Port2.Player2Fire));
    }

    [Fact]
    public void WriteBit_Port2_ClearsMaskedBitsOnly()
    {
        var bus = CreateBus();
        bus.WriteInput(Port2.Player2Fire, true);
        bus.WriteBit(Port2.BonusAt1000, Port2.BonusAt1000);
        bus.WriteBit(Port2.BonusAt1000, 0);
        Assert.Equal(Port2.None, bus.ReadBit(Port2.BonusAt1000));
        Assert.Equal(Port2.Player2Fire, bus.ReadBit(Port2.Player2Fire));
    }

    // --- Port 3 (write): Sound triggers ---

    [Fact]
    public void WritePort3_Shot_WhenBitGoesHigh_CallsShot()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x02);
        Assert.Equal(1, audio.ShotCallCount);
    }

    [Fact]
    public void WritePort3_Shot_WhenBitStaysHigh_DoesNotCallShotAgain()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x02);
        bus.WritePort(3, 0x02);
        Assert.Equal(1, audio.ShotCallCount);
    }

    [Fact]
    public void WritePort3_Shot_WhenBitGoesLow_DoesNotCallShot()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x02);
        bus.WritePort(3, 0x00);
        Assert.Equal(1, audio.ShotCallCount);
    }

    [Fact]
    public void WritePort3_PlayerDie_WhenBitGoesHigh_CallsPlayerDie()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x04);
        Assert.Equal(1, audio.PlayerDieCallCount);
    }

    [Fact]
    public void WritePort3_PlayerDie_WhenBitStaysHigh_DoesNotCallAgain()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x04);
        bus.WritePort(3, 0x04);
        Assert.Equal(1, audio.PlayerDieCallCount);
    }

    [Fact]
    public void WritePort3_PlayerDie_WhenBitGoesLow_DoesNotCallPlayerDie()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x04);
        bus.WritePort(3, 0x00);
        Assert.Equal(1, audio.PlayerDieCallCount);
    }

    [Fact]
    public void WritePort3_InvaderDie_WhenBitGoesHigh_CallsInvaderDie()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x08);
        Assert.Equal(1, audio.InvaderDieCallCount);
    }

    [Fact]
    public void WritePort3_InvaderDie_WhenBitStaysHigh_DoesNotCallAgain()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x08);
        bus.WritePort(3, 0x08);
        Assert.Equal(1, audio.InvaderDieCallCount);
    }

    [Fact]
    public void WritePort3_InvaderDie_WhenBitGoesLow_DoesNotCallInvaderDie()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x08);
        bus.WritePort(3, 0x00);
        Assert.Equal(1, audio.InvaderDieCallCount);
    }

    [Fact]
    public void WritePort3_ExtendedPlay_WhenBitGoesHigh_CallsExtendedPlay()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x10);
        Assert.Equal(1, audio.ExtendedPlayCallCount);
    }

    [Fact]
    public void WritePort3_ExtendedPlay_WhenBitStaysHigh_DoesNotCallAgain()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x10);
        bus.WritePort(3, 0x10);
        Assert.Equal(1, audio.ExtendedPlayCallCount);
    }

    [Fact]
    public void WritePort3_ExtendedPlay_WhenBitGoesLow_DoesNotCallExtendedPlay()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x10);
        bus.WritePort(3, 0x00);
        Assert.Equal(1, audio.ExtendedPlayCallCount);
    }

    [Fact]
    public void WritePort3_UfoLoop_WhenBitGoesHigh_CallsUfoLoopTrue()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x01);
        Assert.Equal(1, audio.UfoLoopCallCount);
        Assert.True(audio.LastUfoLoopActive);
    }

    [Fact]
    public void WritePort3_UfoLoop_WhenBitGoesLow_CallsUfoLoopFalse()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x01);
        bus.WritePort(3, 0x00);
        Assert.Equal(2, audio.UfoLoopCallCount);
        Assert.False(audio.LastUfoLoopActive);
    }

    [Fact]
    public void WritePort3_UfoLoop_WhenBitStaysHigh_DoesNotCallAgain()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(3, 0x01);
        bus.WritePort(3, 0x01);
        Assert.Equal(1, audio.UfoLoopCallCount);
    }

    [Fact]
    public void WritePort3_WhenNoAudio_DoesNotThrow()
    {
        var bus = CreateBus();
        bus.WritePort(3, 0xFF);
    }

    // --- Port 5: Sound triggers ---

    [Fact]
    public void WritePort5_FleetMove1_WhenBitGoesHigh_CallsFleetMove1()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(5, 0x01);
        Assert.Equal([1], audio.FleetMoveSteps);
    }

    [Fact]
    public void WritePort5_FleetMove2_WhenBitGoesHigh_CallsFleetMove2()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(5, 0x02);
        Assert.Equal([2], audio.FleetMoveSteps);
    }

    [Fact]
    public void WritePort5_FleetMove3_WhenBitGoesHigh_CallsFleetMove3()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(5, 0x04);
        Assert.Equal([3], audio.FleetMoveSteps);
    }

    [Fact]
    public void WritePort5_FleetMove4_WhenBitGoesHigh_CallsFleetMove4()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(5, 0x08);
        Assert.Equal([4], audio.FleetMoveSteps);
    }

    [Fact]
    public void WritePort5_FleetMove_WhenBitStaysHigh_DoesNotCallAgain()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(5, 0x01);
        bus.WritePort(5, 0x01);
        Assert.Equal([1], audio.FleetMoveSteps);
    }

    [Fact]
    public void WritePort5_UfoHit_WhenBitGoesHigh_CallsUfoHit()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(5, 0x10);
        Assert.Equal(1, audio.UfoHitCallCount);
    }

    [Fact]
    public void WritePort5_UfoHit_WhenBitStaysHigh_DoesNotCallAgain()
    {
        var bus = CreateBusWithAudio(out var audio);
        bus.WritePort(5, 0x10);
        bus.WritePort(5, 0x10);
        Assert.Equal(1, audio.UfoHitCallCount);
    }

    [Fact]
    public void WritePort5_WhenNoAudio_DoesNotThrow()
    {
        var bus = CreateBus();
        bus.WritePort(5, 0xFF);
    }
}
