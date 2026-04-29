namespace SpaceInvadersEmulator.Core.Intel8080;

public interface IMemoryBus
{
    void Write(ushort address, byte value);
    void WriteWord(ushort address, ushort value);
    byte Read(ushort address);
    ushort ReadWord(ushort address);
}