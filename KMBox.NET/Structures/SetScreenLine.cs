using System.Runtime.InteropServices;

namespace KMBox.NET.Structures;

/// <summary>
/// Screen line buffer packet. Used to update one line of the LCD screen. <br/>
/// Used in <see cref="KmCommand.CmdShowPicture"/>.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct ScreenLineBuffer
{
    [FieldOffset(0)]
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
    public byte[] Buffer;
}