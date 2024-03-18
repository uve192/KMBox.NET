using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace KMBox.NET.Structures;

/// <summary>
/// Header of each command.
/// </summary>
[PublicAPI]
[StructLayout(LayoutKind.Explicit)]
public struct CmdHeadT
{
    /// <summary>
    /// Device's MAC/UUID.
    /// </summary>
    [FieldOffset(0)]
    public uint mac;
    
    /// <summary>
    /// Typically should be filled with random data. <br/>
    /// Sometimes used to pass some kind of value. See <see cref="KmBoxClient.MaskKeyboardButton"/>
    /// </summary>
    [FieldOffset(4)]
    public uint rand;
    
    /// <summary>
    /// Command index. Should be incremented with each sent command.
    /// </summary>
    [FieldOffset(8)]
    public uint indexpts;
    
    /// <summary>
    /// Command ID.
    /// </summary>
    [FieldOffset(12)]
    public KmCommand cmd;
}