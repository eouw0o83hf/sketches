using System.Runtime.InteropServices;
using SkiaSharp;

namespace Sketches.Console;

public static class Utilities
{
    /// <summary>
    /// Turns a 3D array [row, col, [red, green, blue]] into a Skia bitmap
    /// </summary>
    /// <source>
    /// https://swharden.com/csdv/skiasharp/array-to-image/
    /// </source>
    public static SKBitmap ArrayToImage(byte[,,] pixelArray)
    {
        int width = pixelArray.GetLength(1);
        int height = pixelArray.GetLength(0);

        uint[] pixelValues = new uint[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                byte alpha = 255;
                // TODO i think BG may be swapped since we have to use BGRA for the bitmap->video adapter
                byte red = pixelArray[y, x, 0];
                byte green = pixelArray[y, x, 1];
                byte blue = pixelArray[y, x, 2];
                uint pixelValue = (uint)red + (uint)(green << 8) + (uint)(blue << 16) + (uint)(alpha << 24);
                pixelValues[y * width + x] = pixelValue;
            }
        }

        return ArrayToImage1D(width, height, pixelValues);
    }

    public static SKBitmap ArrayToImage1D(int width, int height, uint[] pixelArray)
    {
        SKBitmap bitmap = new();
        GCHandle gcHandle = GCHandle.Alloc(pixelArray, GCHandleType.Pinned);
        SKImageInfo info = new(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);

        IntPtr ptr = gcHandle.AddrOfPinnedObject();
        int rowBytes = info.RowBytes;
        bitmap.InstallPixels(info, ptr, rowBytes, delegate { gcHandle.Free(); });

        return bitmap;
    }
}
