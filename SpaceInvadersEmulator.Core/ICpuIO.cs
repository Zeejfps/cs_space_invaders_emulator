namespace SpaceInvadersEmulator.Core;

public interface ICpuIO
{
    void WritePort(byte port, byte value);
    byte ReadPort(byte port);
}