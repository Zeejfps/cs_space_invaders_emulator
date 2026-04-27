using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SpaceInvadersEmulator.Core;

public sealed class Mmu
{
    public const ushort RomBlock = 0x2000;
    
    private readonly byte[] _ram = new byte[64*1024];
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Write(ushort address, byte value)
    {
        if (address < RomBlock) return;
        Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_ram), address) = value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Write(ushort address, ReadOnlySpan<byte> value)
    {
        if (address < RomBlock) return;
        ref var dst = ref Unsafe.Add(ref                
            MemoryMarshal.GetArrayDataReference(_ram), address);  
        ref var src = ref
            MemoryMarshal.GetReference(value);                    
        Unsafe.CopyBlockUnaligned(ref dst, ref src,     
            (uint)value.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void WriteWord(ushort address, ushort value)
    {
        Write(address, (byte)(value & 0xFF));
        Write((ushort)(address + 1), (byte)(value >> 8));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public byte Read(ushort address)
    {
        return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_ram), address);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public ushort ReadWord(ushort address)
    {
        var lo = Read(address);
        var hi = Read((ushort)(address + 1));
        return (ushort)((hi << 8) | lo);
    }
}