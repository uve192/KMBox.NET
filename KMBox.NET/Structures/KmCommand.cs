namespace KMBox.NET.Structures;

/// <summary>
/// KMBox commands.
/// </summary>
public enum KmCommand : uint
{
    /// <summary>
    /// Command to do a handshake with KMBox and initiate connection.
    /// </summary>
    CmdConnect = 0xaf3c2828,
    
    /// <summary>
    /// Command to move the mouse (simple mode).
    /// </summary>
    CmdMouseMove = 0xaede7345,
    
    /// <summary>
    /// Command to press left mouse button. Also works for middle/right for some reason.
    /// </summary>
    CmdMouseLeft = 0x9823AE8D,
    
    /// <summary>
    /// Unused in this library.
    /// </summary>
    CmdMouseMiddle = 0x97a3AE8D,
    
    /// <summary>
    /// Unused in this library.
    /// </summary>
    CmdMouseRight = 0x238d8212,
    
    /// <summary>
    /// Move mouse wheel.
    /// </summary>
    CmdMouseWheel = 0xffeead38,
    
    /// <summary>
    /// Command to smoothly move your mouse.
    /// </summary>
    CmdMouseAutomove = 0xaede7346,
    
    /// <summary>
    /// All keyboard actions.
    /// </summary>
    CmdKeyboardAll = 0x123c2c2f,
    
    /// <summary>
    /// Reboot KMBox.
    /// </summary>
    CmdReboot = 0xaa8855aa,
    
    /// <summary>
    /// Move mouse using <a href="https://en.wikipedia.org/wiki/B%C3%A9zier_curve">Bézier curve</a>
    /// </summary>
    CmdBeizerMove = 0xa238455a,
    
    /// <summary>
    /// Command to enable/disable monitor.
    /// </summary>
    CmdMonitor = 0x27388020,
    
    /// <summary>
    /// I have no idea!
    /// </summary>
    CmdDebug = 0x27382021,
    
    /// <summary>
    /// Command to enable/disable keyboard/mouse mask. Basically disables input.
    /// </summary>
    CmdMask = 0x23234343,
    
    /// <summary>
    /// Command to disable all active masks.
    /// </summary>
    CmdUnmaskAll = 0x23344343,
    
    /// <summary>
    /// Command to configure KMBox's IP and port.
    /// </summary>
    CmdSetConfig = 0x1d3d3323,
    
    /// <summary>
    /// Command to show a picture on screen.
    /// </summary>
    CmdShowPicture = 0x12334883
}