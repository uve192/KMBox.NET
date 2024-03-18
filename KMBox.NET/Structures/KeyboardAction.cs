using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace KMBox.NET.Structures;

/// <summary>
/// Keyboard action packet.
/// </summary>
[PublicAPI]
[StructLayout(LayoutKind.Explicit, Size = 12)]
public struct KeyboardAction
{
    /// <summary>
    /// Control flags. Shift, Ctrl, Alt, etc..
    /// </summary>
    [FieldOffset(0)]
    public byte Ctrl;

    /// <summary>
    /// No clue! Unused?
    /// </summary>
    [FieldOffset(1)]
    public byte ResVel;

    // Unaligned array god damnit you marshaller

    /// <summary>
    /// The first button to press.
    /// </summary>
    [FieldOffset(2)]
    public byte Button0;

    /// <summary>
    /// The second button to press.
    /// </summary>
    [FieldOffset(3)]
    public byte Button1;

    /// <summary>
    /// The third button to press.
    /// </summary>
    [FieldOffset(4)]
    public byte Button2;

    /// <summary>
    /// The fourth button to press.
    /// </summary>
    [FieldOffset(5)]
    public byte Button3;

    /// <summary>
    /// The fifth button to press.
    /// </summary>
    [FieldOffset(6)]
    public byte Button4;

    /// <summary>
    /// The sixth button to press.
    /// </summary>
    [FieldOffset(7)]
    public byte Button5;

    /// <summary>
    /// The seventh button to press.
    /// </summary>
    [FieldOffset(8)]
    public byte Button6;

    /// <summary>
    /// The eighth button to press.
    /// </summary>
    [FieldOffset(9)]
    public byte Button7;

    /// <summary>
    /// The ninth button to press.
    /// </summary>
    [FieldOffset(10)]
    public byte Button8;

    /// <summary>
    /// The tenth button to press.
    /// </summary>
    [FieldOffset(11)]
    public byte Button9;

    /// <summary>
    /// Sets first button to press. To send more buttons in one command please look at: <br/>
    /// <see cref="Button1"/>, <see cref="Button2"/>, <see cref="Button3"/>, etc..
    /// </summary>
    /// <param name="button"></param>
    /// <param name="modifiers"></param>
    public void SetButtons(KeyboardButton button, KeyboardModifiers modifiers = 0)
    {
        Button0 = (byte) button;
        Ctrl = (byte) modifiers;
    }

    /// <summary>
    /// Resets all buttons in this command.
    /// </summary>
    public void ResetButtons()
    {
        Button0 = 0;
        Button1 = 0;
        Button2 = 0;
        Button3 = 0;
        Button4 = 0;
        Button5 = 0;
        Button6 = 0;
        Button7 = 0;
        Button8 = 0;
        Button9 = 0;
    }
}