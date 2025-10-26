using System.Numerics;

namespace Sketches.Console.Scenes.Mandelbrot;

public class Mandelbrot(int _height, int _width, double _scale) : IRenderable
{
    public uint[] Render(float t)
    {
        var scale = _scale - (t * 0.000004);

        var yCenter = _height / 2;
        var xCenter = (_width / 4);
        var result = new uint[_height * _width];

        Parallel.For(0, _height, i =>
        // for (var i = 0; i < _height; ++i)
        {
            for (var j = 0; j < _width; ++j)
            {
                var y = (i - yCenter) * scale;
                var x = (j - xCenter) * scale;
                var c = new Complex(x, y);

                result[i * _width + j] = m(c).RGBA;
            }
        });

        return result;
    }

    private Color m(Complex c)
    {
        var d = Divergence(c);
        if (d == -1)
        {
            return new Color(0, 0, 0);
        }
        var str = (byte)Math.Min(d * 5, 0xff);
        return new Color(str, str, str);
    }

    private int Divergence(Complex c)
    {
        var acc = Complex.Zero;
        var max = 100;
        var escapeMag = 10;
        for (var i = 0; i <= max; ++i)
        {
            acc = f(acc, c);

            if (acc.Magnitude > escapeMag)
            {
                return i;
            }
        }
        return -1;
    }

    // doesn't work yet but it's cool glitch art
    private int Divergence2(Complex c)
    {
        var asc = 0;
        var acc = Complex.Zero;
        var prev = acc;

        for (var i = 0; i < 100; ++i)
        {
            acc = f(acc, c);
            if (acc.Magnitude > prev.Magnitude)
            {
                ++asc;
                if (asc >= 4)
                {
                    return i;
                }
            }
            else
            {
                asc = 0;
            }
            prev = acc;
        }

        return -1;
    }

    private Complex f(Complex z, Complex c)
        => Complex.Pow(z, 2) + c;
}
