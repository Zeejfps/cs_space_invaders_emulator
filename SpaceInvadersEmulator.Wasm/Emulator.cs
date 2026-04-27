using System.Buffers;
using System.Runtime.InteropServices.JavaScript;
using SpaceInvadersEmulator.Core;

namespace SpaceInvadersEmulator.Wasm;

public static partial class Emulator
{
    private static BrowserClock? _clock;
    private static BrowserAudio? _audio;
    private static Machine? _machine;

    private static MemoryHandle _vramHandle;
    private static int _vramPtrValue;
    private static int _vramLen;

    [JSExport]
    public static unsafe void Initialize()
    {
        _clock = new BrowserClock();
        _audio = new BrowserAudio();
        _machine = new Machine(_clock, _audio);

        _vramHandle = _machine.VRam.Pin();
        _vramPtrValue = (int)_vramHandle.Pointer;
        _vramLen = _machine.VRam.Length;
    }

    [JSExport]
    public static void LoadRom(byte[] data)
    {
        _machine!.LoadRom(data);
        _machine.Start();
    }

    /// <summary>
    /// Advances the emulator by one frame. After this returns, the VRam region
    /// (accessible via GetVRamPtr/GetVRamLen from HEAPU8) reflects the new frame.
    /// </summary>
    [JSExport]
    public static void RunFrame() => _clock!.Tick();

    /// <summary>
    /// Absolute WASM linear-memory address of the start of the pinned VRam region.
    /// Use Module.HEAPU8.subarray(GetVRamPtr(), GetVRamPtr() + GetVRamLen()) for a zero-copy view.
    /// </summary>
    [JSExport]
    public static int GetVRamPtr() => _vramPtrValue;

    [JSExport]
    public static int GetVRamLen() => _vramLen;

    [JSExport]
    public static void WriteP1Left(bool pressed) => _machine!.WriteP1Left(pressed);

    [JSExport]
    public static void WriteP1Right(bool pressed) => _machine!.WriteP1Right(pressed);

    [JSExport]
    public static void WriteP1Fire(bool pressed) => _machine!.WriteP1Fire(pressed);

    [JSExport]
    public static void WriteCoin(bool pressed) => _machine!.WriteCoin(pressed);

    [JSExport]
    public static void WriteP1Start(bool pressed) => _machine!.WriteP1Start(pressed);

    [JSExport]
    public static void WriteP2Start(bool pressed) => _machine!.WriteP2Start(pressed);

    [JSExport]
    public static void WriteP2Fire(bool pressed) => _machine!.WriteP2Fire(pressed);

    [JSExport]
    public static void WriteP2Left(bool pressed) => _machine!.WriteP2Left(pressed);

    [JSExport]
    public static void WriteP2Right(bool pressed) => _machine!.WriteP2Right(pressed);

    [JSExport]
    public static void SetShips(int count)
    {
        _machine!.Ships = count switch
        {
            4 => ShipCount.Four,
            5 => ShipCount.Five,
            6 => ShipCount.Six,
            _ => ShipCount.Three,
        };
    }

    [JSExport]
    public static void SetBonusLife(bool at1000)
    {
        _machine!.BonusLife = at1000 ? BonusLifeThreshold.At1000 : BonusLifeThreshold.At1500;
    }
}
