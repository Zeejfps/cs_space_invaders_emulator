using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpaceInvadersEmulator.Core;

public sealed class Mmu
{
    private readonly byte[] _ram = new byte[64*1024];
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ushort address, byte value)
    {
        Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_ram), address) = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte Read(ushort address)
    {
        return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_ram), address);
    }
}