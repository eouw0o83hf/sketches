using System.Numerics;
using System.Security.Cryptography;

namespace Sketches.Console;

/// <summary>
/// Fractional Brownian Motion
/// </summary>
/// <source>
/// https://iquilezles.org/articles/fbm/
/// </source>
public class Fbm
{
    float fbm(Vector3 x, float H)
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

    float noise(in Vector3 x)
    {
        var p = new Vector3(float.Floor(x[0]), float.Floor(x[1]), float.Floor(x[2]));
        var w = x - p;

        var u = w * w * w * (w * (Vector3.Multiply(6, w) - fifteen) + ten);
        float n = p[0] + 317.0f * p[1] + 157.0f * p[2];

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

        return -1.0f + 2.0f * (k0 + k1 * u[0] + k2 * u[1] + k3 * u[2] + k4 * u[0] * u[1] + k5 * u[1] * u[2] + k6 * u[2] * u[0] + k7 * u[0] * u[1] * u[2]);
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

    float hash1(Vector2 p)
    {
        var tmp = Vector2.Multiply(0.3183099f, p);
        var floor = new Vector2(float.Floor(tmp[0]), float.Floor(tmp[1]));
        var fractional = tmp - floor;

        p = Vector2.Multiply(50.0f, fractional);

        return fract(p[0] * p[1] * (p[0] + p[1]));
    }

    float hash1(float n)
    {
        return fract(n * 17.0f * fract(n * 0.3183099f));
    }

    float fract(float n) => n - float.Floor(n);
}
