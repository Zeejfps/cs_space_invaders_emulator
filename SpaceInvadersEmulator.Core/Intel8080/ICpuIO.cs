namespace SpaceInvadersEmulator.Core.Intel8080;

public interface ICpuIO
{
    void WritePort(byte port, byte value);
    byte ReadPort(byte port);
}