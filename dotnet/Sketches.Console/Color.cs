using SkiaSharp;

namespace Sketches.Console;

public struct Color(byte red, byte green, byte blue, byte alpha = 0xff)
{
    public byte Red { get; set; } = red;
    public byte Green { get; set; } = green;
    public byte Blue { get; set; } = blue;
    public byte Alpha { get; set; } = alpha;

    public readonly SKColor ToSKColor()
        => new(Red, Green, Blue, Alpha);

    public static Color FromRgba(uint rgba)
    {
        var acc = rgba;
        var alpha = (byte)(acc & 0xff);
        acc >>= 8;
        var blue = (byte)(acc & 0xff);
        acc >>= 8;
        var green = (byte)(acc & 0xff);
        acc >>= 8;
        var red = (byte)(acc & 0xff);

        return new Color(red, green, blue, alpha);
    }

    public readonly uint RGBA => (uint)Red << 24 | (uint)Green << 16 | (uint)Blue << 8 | (uint)Alpha;
}
