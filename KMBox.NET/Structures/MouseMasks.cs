namespace KMBox.NET.Structures;

/// <summary>
/// Mouse masks. Used to disable certain kind of input. <br/>
/// Combinable.
/// </summary>
[Flags]
public enum MouseMasks
{
    /// <summary>
    /// Disable left click.
    /// </summary>
    Left = 1 << 0,
    
    /// <summary>
    /// Disable right click.
    /// </summary>
    Right = 1 << 1,
    
    /// <summary>
    /// Disable middle click.
    /// </summary>
    Middle = 1 << 2,
    
    /// <summary>
    /// Disable first side button click.
    /// </summary>
    Side1 = 1 << 3,
    
    /// <summary>
    /// Disable second side button click.
    /// </summary>
    Side2 = 1 << 4,
    
    /// <summary>
    /// Disable movement on X axis.
    /// </summary>
    XMovement = 1 << 5,
    
    /// <summary>
    /// Disable movement on Y axis.
    /// </summary>
    YMovement = 1 << 6,
    
    /// <summary>
    /// Disable wheel scroll.
    /// </summary>
    Wheel = 1 << 7
}