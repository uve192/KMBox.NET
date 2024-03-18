using System.Runtime.InteropServices;

namespace KMBox.NET.Structures;

/// <summary>
/// Command that sets KMBox's IP and port. <br/>
/// IP address is set in the <see cref="CmdHeadT.rand"/> field in network byte order: 127.0.0.1 -> [127, 0, 0, 1].
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct SetConfigCommand
{
    public SetConfigCommand()
    {
    }
    
    /// <summary>
    /// Port field. Must be little endian, p sure :)
    /// </summary>
    [FieldOffset(0)]
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Port = new byte[2];
}