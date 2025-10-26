using FFMpegCore;
using FFMpegCore.Pipes;
using Instances.Exceptions;
using SkiaSharp;
using SConsole = System.Console;

namespace Sketches.Console;

public class RendererOptions
{
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 600;

    /// <summary>
    /// For video, number of frames
    /// </summary>
    public int Length { get; set; } = 0;

    public string Filename { get; set; } = $"render_{DateTime.Now:yyyyMMdd_HHmmss}";
}

public class Renderer(string _path)
{
    public void RenderImageToFile(IRenderable src, RendererOptions opts)
    {
        var filename = Path.ChangeExtension(opts.Filename, ".png");
        var path = Path.Combine(_path, filename);
        var frame = src.Render(0);

        var paints = new Dictionary<uint, SKPaint>();
        using var bm = new SKBitmap(opts.Width, opts.Height);
        using var canvas = new SKCanvas(bm);
        canvas.Clear(SKColors.Black);

        for (var i = 0; i < opts.Height; ++i)
        {
            for (var j = 0; j < opts.Width; ++j)
            {
                var c = frame[i * opts.Width + j];
                if (!paints.TryGetValue(c, out var paint))
                {
                    paint = new SKPaint
                    {
                        Color = new SKColor(c)
                    };
                    paints[c] = paint;
                }

                canvas.DrawPoint(j, i, paint);
            }
        }

        using var img = SKImage.FromBitmap(bm);
        using var png = img.Encode(SKEncodedImageFormat.Png, 100);
        using var fs = File.OpenWrite(path);
        png.SaveTo(fs);
    }

    public void RenderVideoToFile(IRenderable src, RendererOptions opts)
    {
        var filename = Path.ChangeExtension(opts.Filename, ".webm");
        var path = Path.Combine(_path, filename);

        var frames = RenderFrames(src, opts);
        var pipe = new RawVideoPipeSource(frames)
        {
            FrameRate = 30
        };

        try
        {
            var success = FFMpegArguments
                .FromPipeInput(pipe)
                .OutputToFile(path, overwrite: true, o => o.WithVideoCodec("libvpx-vp9"))
                .ProcessSynchronously();
        }
        catch (InstanceFileNotFoundException ex)
        {
            throw new Exception("You need to install ffmpeg, visit https://www.ffmpeg.org/download.html", ex);
        }
    }

    private static IEnumerable<IVideoFrame> RenderFrames(IRenderable src, RendererOptions opts)
    {
        for (int t = 0; t < opts.Length; t++)
        {
            SConsole.WriteLine($"Rendering frame {t + 1} of {opts.Length}");

            var frame = src.Render(t);
            var bm = Utilities.ArrayToImage1D(opts.Width, opts.Height, frame);

            using var result = new SKBitmapFrame(bm);
            yield return result;
        }

        SConsole.WriteLine("Rendering complete! 🎉");
    }
}
