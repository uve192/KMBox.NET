using JetBrains.Annotations;

namespace KMBox.NET.Structures;

/// <summary>
/// Mouse button flags.
/// </summary>
[Flags]
[PublicAPI]
public enum MouseButton
{
    /// <summary>
    /// Left mouse button.
    /// </summary>
    MouseLeft = 1,
    
    /// <summary>
    /// Right mouse button.
    /// </summary>
    MouseRight = 1 << 1,
    
    /// <summary>
    /// Middle mouse button.
    /// </summary>
    MouseMiddle = 1 << 2
}
