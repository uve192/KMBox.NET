using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace KMBox.NET.Structures;

/// <summary>
/// Mouse monitor report.
/// </summary>
[PublicAPI]
[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct MouseReport
{
    /// <summary>
    /// ID of the report.
    /// </summary>
    [FieldOffset(0)]
    public byte ReportId;

    /// <summary>
    /// Currently pressed buttons.
    /// </summary>
    [FieldOffset(1)]
    public byte Buttons;

    /// <summary>
    /// Current movement on X axis.
    /// </summary>
    [FieldOffset(2)]
    public short X;

    /// <summary>
    /// Current movement on Y axis.
    /// </summary>
    [FieldOffset(4)]
    public short Y;

    /// <summary>
    /// Current scroll.
    /// </summary>
    [FieldOffset(6)]
    public short Wheel;

    public override string ToString() =>
        $"{ReportId}: Buttons={Buttons}, X={X}, Y={Y}, Wheel={Wheel}";
}

/// <summary>
/// Keyboard monitor report.
/// </summary>
[PublicAPI]
[StructLayout(LayoutKind.Explicit, Size = 12)]
public struct KeyboardReport
{
    /// <summary>
    /// ID of the report.
    /// </summary>
    [FieldOffset(0)]
    public byte ReportId;

    /// <summary>
    /// Not sure, actually. Modifier buttons?
    /// </summary>
    [FieldOffset(1)]
    public byte Buttons;

    /// <summary>
    /// First key pressed.
    /// </summary>
    [FieldOffset(2)]
    public byte FirstKey;

    /// <summary>
    /// Second button pressed.
    /// </summary>
    [FieldOffset(3)]
    public byte SecondButton;

    /// <summary>
    /// Third button pressed.
    /// </summary>
    [FieldOffset(4)]
    public byte ThirdButton;

    /// <summary>
    /// Fourth button pressed.
    /// </summary>
    [FieldOffset(5)]
    public byte FourthButton;

    /// <summary>
    /// Fifth button pressed.
    /// </summary>
    [FieldOffset(6)]
    public byte FifthButton;

    /// <summary>
    /// Sixth button pressed.
    /// </summary>
    [FieldOffset(7)]
    public byte SixthButton;

    /// <summary>
    /// Seventh button pressed.
    /// </summary>
    [FieldOffset(8)]
    public byte SeventhButton;

    /// <summary>
    /// Eighth button pressed.
    /// </summary>
    [FieldOffset(9)]
    public byte EighthButton;

    /// <summary>
    /// Ninth button pressed.
    /// </summary>
    [FieldOffset(10)]
    public byte NinthButton;

    /// <summary>
    /// Tenth button pressed.
    /// </summary>
    [FieldOffset(11)]
    public byte TenthButton;

    public bool IsButtonPressed(KeyboardButton button)
{
    var val = (byte) button;
    
    return FirstKey == val || 
           SecondButton == val ||
           ThirdButton == val ||
           FourthButton == val ||
           FifthButton == val ||
           SixthButton == val ||
           SeventhButton == val ||
           EighthButton == val ||
           NinthButton == val ||
           TenthButton == val;
}

    public override string ToString() =>
        $"{ReportId}: Buttons=({FirstKey}, {SecondButton}, {ThirdButton}, {FourthButton}, {FifthButton}, {SixthButton}, {SeventhButton}, {EighthButton}, {NinthButton}, {TenthButton})";
}

/// <summary>
/// Monitor report packet.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct CompositeReport
{
    /// <summary>
    /// See <see cref="MouseReport"/>.
    /// </summary>
    [FieldOffset(0)]
    public MouseReport MouseReport;

    /// <summary>
    /// See <see cref="KeyboardReport"/>.
    /// </summary>
    [FieldOffset(8)]
    public KeyboardReport KeyboardReport;

    public override string ToString() => 
        $"{MouseReport}\n{KeyboardReport}";
}