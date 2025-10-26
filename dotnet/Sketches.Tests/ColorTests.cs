using Sketches.Console;

namespace Sketches.Tests;

public class ColorTests
{
    [Theory]
    [InlineData(0xaa, 0xbb, 0xcc, 0xdd, 0xaabbccdd)]
    public void Rgba_Get(byte r, byte g, byte b, byte a, uint rgba)
        => Assert.Equal(rgba, new Color(r, g, b, a).RGBA);
}
