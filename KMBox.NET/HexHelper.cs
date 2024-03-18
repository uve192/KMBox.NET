namespace KMBox.NET;

/// <summary>
/// HEX string helper.
/// </summary>
public static class HexHelper
{
    /// <summary>
    /// Converts KMBox's UUID/MAC to <see cref="uint"/>.
    /// </summary>
    /// <param name="pbSrc">KMBox's UUID/MAC.</param>
    public static uint MacToUInt(string pbSrc)
    {
        var pbDest = new byte[16];
        
        for (var i = 0; i < pbSrc.Length / 2; i++)
        {
            var h1 = Convert.ToByte(pbSrc[2 * i]);
            var h2 = Convert.ToByte(pbSrc[2 * i + 1]);
            
            var s1 = (byte)(char.ToUpper((char)h1) - 0x30);
            
            if (s1 > 9)
                s1 -= 7;
            
            var s2 = (byte)(char.ToUpper((char)h2) - 0x30);
            
            if (s2 > 9)
                s2 -= 7;
            
            pbDest[i] = (byte)(s1 * 16 + s2);
        }

        return (uint)((pbDest[0] << 24) | (pbDest[1] << 16) | (pbDest[2] << 8) | pbDest[3]);
    }
}