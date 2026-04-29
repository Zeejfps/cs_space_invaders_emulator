namespace SpaceInvadersEmulator.Core.Intel8080;

public interface IIOBus
{
    void WritePort(byte port, byte value);
    byte ReadPort(byte port);
}