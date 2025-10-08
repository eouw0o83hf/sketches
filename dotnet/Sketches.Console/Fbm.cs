using System.Numerics;

namespace Sketches.Console;

/// <summary>
/// Fractional Brownian Motion
/// </summary>
/// <source>
/// https://iquilezles.org/articles/fbm/
/// </source>
public class Fbm
{
    public static float Pattern3(Vector2 p, float? z = null)
    {
        var q = new Vector2(fbm(p + new Vector2(0.0f, 0.0f), z),
                       fbm(p + new Vector2(5.2f, 1.3f), z));

        var r = new Vector2(fbm(p + 4.0f * q + new Vector2(1.7f, 9.2f), z),
                       fbm(p + 4.0f * q + new Vector2(8.3f, 2.8f), z));

        return fbm(p + 4.0f * r, z);
    }

    public static float Pattern2(Vector2 p)
    {
        var q = new Vector2(fbm(p + new Vector2(0.0f, 0.0f)),
                       fbm(p + new Vector2(5.2f, 1.3f)));

        return fbm(p + Vector2.Multiply(4.0f, q));
    }

    public static float Pattern(Vector2 p)
        => fbm(new Vector3(p.X, p.Y, 0), 1);

    static float fbm(Vector2 x, float? z = null)
        => fbm(new Vector3(x.X, x.Y, z ?? 0f), 1f);

    static float fbm(Vector3 x, float H)
    {
        var numOctaves = 3;
        float G = (float)Math.Pow(2f, -H);
        float f = 1.0f;
        float a = 1.0f;
        float t = 0.0f;
        for (int i = 0; i < numOctaves; i++)
        {
            t += a * noise(Vector3.Multiply(f, x));
            f *= 2.0f;
            a *= G;
        }
        return t;
    }


    private static readonly Vector3 fifteen = new Vector3(15f, 15f, 15f);
    private static readonly Vector3 ten = new Vector3(10f, 10f, 10f);
    private static readonly Vector3 two = new Vector3(2, 2, 2);
    private static readonly Vector3 one = new Vector3(1, 1, 1);

    static float noise(in Vector3 x)
    {
        var p = new Vector3(float.Floor(x.X), float.Floor(x.Y), float.Floor(x.Z));
        var w = x - p;

        var u = w * w * w * (w * (Vector3.Multiply(6, w) - fifteen) + ten);
        float n = p.X + 317.0f * p.Y + 157.0f * p.Z;

        float a = hash1(n + 0.0f);
        float b = hash1(n + 1.0f);
        float c = hash1(n + 317.0f);
        float d = hash1(n + 318.0f);
        float e = hash1(n + 157.0f);
        float f = hash1(n + 158.0f);
        float g = hash1(n + 474.0f);
        float h = hash1(n + 475.0f);

        float k0 = a;
        float k1 = b - a;
        float k2 = c - a;
        float k3 = e - a;
        float k4 = a - b - c + d;
        float k5 = a - c - e + g;
        float k6 = a - b - e + f;
        float k7 = -a + b + c - d + e - f - g + h;

        return -1.0f + 2.0f * (k0 + k1 * u.X + k2 * u.Y + k3 * u.Z + k4 * u.X * u.Y + k5 * u.Y * u.Z + k6 * u.Z * u.X + k7 * u.X * u.Y * u.Z);
    }

    // double ValueNoise(Vector3 x)
    // {
    //     var p = new Vector3(float.Floor(x[0]), float.Floor(x[1]), float.Floor(x[2]));
    //     var w = x - p;

    //     var u = w * w * w * (w * (Vector3.Multiply(6, w) - fifteen) + ten);
    //     var du = Vector3.Multiply(30.0f, w) * w * (w * (w - one) + two);

    //     float a = hash1(p + new Vector3(0, 0, 0));
    //     float b = hash1(p + new Vector3(1, 0, 0));
    //     float c = hash1(p + new Vector3(0, 1, 0));
    //     float d = hash1(p + new Vector3(1, 1, 0));
    //     float e = hash1(p + new Vector3(0, 0, 1));
    //     float f = hash1(p + new Vector3(1, 0, 1));
    //     float g = hash1(p + new Vector3(0, 1, 1));
    //     float h = hash1(p + new Vector3(1, 1, 1));

    //     float k0 = a;
    //     float k1 = b - a;
    //     float k2 = c - a;
    //     float k3 = e - a;
    //     float k4 = a - b - c + d;
    //     float k5 = a - c - e + g;
    //     float k6 = a - b - e + f;
    //     float k7 = -a + b + c - d + e - f - g + h;

    //     return -1.0 + 2.0 * (k0 + k1 * u.x + k2 * u.y + k3 * u.z + k4 * u.x * u.y + k5 * u.y * u.z + k6 * u.z * u.x + k7 * u.x * u.y * u.z;
    // }

    static float hash1(Vector2 p)
    {
        var tmp = Vector2.Multiply(0.3183099f, p);
        var floor = new Vector2(float.Floor(tmp.X), float.Floor(tmp.Y));
        var fractional = tmp - floor;

        p = Vector2.Multiply(50.0f, fractional);

        return fract(p.X * p.Y * (p.X + p.Y));
    }

    static float hash1(float n)
    {
        return fract(n * 17.0f * fract(n * 0.3183099f));
    }

    static float fract(float n) => n - float.Floor(n);
}
