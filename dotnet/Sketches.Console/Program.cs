// See https://aka.ms/new-console-template for more information
using SkiaSharp;
using FFMpegCore;
using FFMpegCore.Pipes;
using Instances.Exceptions;
using Sketches.Console;
using System.Numerics;
using FFMpegCore.Arguments;


internal class Program
{
    public static void Main_NoisePic(string[] args)
    {
        var width = 1200;
        var height = 800;
        var scale = 1 / 1000f;

        using var bm = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bm);

        canvas.Clear(SKColors.Black);
        for (var i = 0; i < width; ++i)
        {
            for (var j = 0; j < height; ++j)
            {
                var color = Fbm.Pattern3(new Vector2((float)i * scale, (float)j * scale));
                var paint = new SKPaint
                {
                    Color = new SKColor(255, 255, 255, (byte)(255 * color))
                };
                canvas.DrawPoint(i, j, paint);
            }
        }

        using var img = SKImage.FromBitmap(bm);
        using var skdata = img.Encode(SKEncodedImageFormat.Png, 100);
        var workingDir = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
        var path = Path.Combine(workingDir, "renders/noise.png");
        using var fs = File.OpenWrite(path);
        skdata.SaveTo(fs);
    }

    private static IEnumerable<IVideoFrame> NoiseFrames(int count, int width, int height)
    {
        var scale = 1 / 1000f;
        for (int t = 0; t < count; t++)
        {
            var frame = new byte[height, width, 3];
            Console.WriteLine($"\rRendering frame {t + 1} of {count}");

            // using var bm = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
            // using var canvas = new SKCanvas(bm);

            // canvas.Clear(SKColors.Black);
            for (var i = 0; i < width; ++i)
            {
                for (var j = 0; j < height; ++j)
                {
                    var sample = Fbm.Pattern3(new Vector2((float)i * scale, (float)j * scale), t * scale);
                    var rgb = (byte)(sample * 255);
                    frame[j, i, 0] = rgb;
                    frame[j, i, 1] = rgb;
                    frame[j, i, 2] = rgb;

                    // var paint = new SKPaint
                    // {
                    //     Color = new SKColor(255, 255, 255, (byte)(255 * color))
                    // };
                    // canvas.DrawPoint(i, j, paint);
                }
            }
            var bm = Utilities.ArrayToImage(frame);

            using SKBitmapFrame result = new(bm);
            yield return result;
        }
    }

    private static void Main(string[] args)
    {
        // using var bm = new SKBitmap(200, 100);
        // using var canvas = new SKCanvas(bm);

        // canvas.Clear(SKColors.Red);
        // for (var i = 0; i < 100; ++i)
        // {
        //     canvas.DrawPoint(i * 2, i, SKColors.AliceBlue);
        // }

        // using var img = SKImage.FromBitmap(bm);
        // using var skdata = img.Encode(SKEncodedImageFormat.Png, 100);

        var workingDir = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
        // var path = Path.Combine(workingDir, "renders/tmp.png");
        // using var fs = File.OpenWrite(path);
        // skdata.SaveTo(fs);

        var videoPath = Path.Combine(workingDir, "renders/noisevideo.webm");

        var frames = NoiseFrames(count: 10, width: 1200, height: 800);
        RawVideoPipeSource videoFramesSource = new(frames) { FrameRate = 30 };
        try
        {
            bool success = FFMpegArguments
                .FromPipeInput(videoFramesSource)
                .OutputToFile(videoPath, overwrite: true, options => options.WithVideoCodec("libvpx-vp9"))
                .ProcessSynchronously();
        }
        catch (InstanceFileNotFoundException ex)
        {
            throw new Exception("You need to install ffmpeg, visit https://www.ffmpeg.org/download.html", ex);
        }

    }

    private static IEnumerable<IVideoFrame> CreateFrames(int count, int width, int height)
    {
        using SKFont textFont = new(SKTypeface.FromFamilyName("consolas"), size: 32);
        using SKPaint textPaint = new(textFont) { Color = SKColors.Yellow, TextAlign = SKTextAlign.Center };
        using SKPaint rectanglePaint = new() { Color = SKColors.Green, Style = SKPaintStyle.Fill };
        SKColor backgroundColor = SKColors.Navy;

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"\rRendering frame {i + 1} of {count}");
            using SKBitmap bmp = new(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
            using SKCanvas canvas = new(bmp);
            canvas.Clear(backgroundColor);
            canvas.DrawRect(i, i, i * 2, i * 2, rectanglePaint);
            canvas.DrawText("SkiaSharp", bmp.Width / 2, bmp.Height * .4f, textPaint);
            canvas.DrawText($"Frame {i}", bmp.Width / 2, bmp.Height * .6f, textPaint);

            using SKBitmapFrame frame = new(bmp);
            yield return frame;
        }
    }
}

/// <summary>
/// This class is used to convert SKBitmap images to IVideoFrame objects that can be used by ffmpeg.
/// </summary>
/// <source>
/// https://swharden.com/csdv/skiasharp/video/
/// </source>
internal class SKBitmapFrame : IVideoFrame, IDisposable
{
    public int Width => Source.Width;
    public int Height => Source.Height;
    public string Format => "bgra";

    private readonly SKBitmap Source;

    public SKBitmapFrame(SKBitmap bmp)
    {
        if (bmp.ColorType != SKColorType.Bgra8888)
            throw new NotImplementedException("only 'bgra' color type is supported");
        Source = bmp;
    }

    public void Dispose() =>
        Source.Dispose();

    public void Serialize(Stream pipe) =>
        pipe.Write(Source.Bytes, 0, Source.Bytes.Length);

    public Task SerializeAsync(Stream pipe, CancellationToken token) =>
        pipe.WriteAsync(Source.Bytes, 0, Source.Bytes.Length, token);
}
