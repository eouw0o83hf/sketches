using FFMpegCore;
using FFMpegCore.Pipes;
using Instances.Exceptions;
using Sketches.Console;
using System.Numerics;
using Sketches.Console.Scenes.Mandelbrot;

internal class Program
{
    private static void Main(string[] args)
    {
        var workingDir = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
        var rendersDir = Path.Combine(workingDir, "renders");

        var opts = new RendererOptions
        {
            Width = 1200,
            Height = 800,
            Length = 10,
            Filename = $"mandelbrot_{DateTime.Now:yyyyMMdd_HHmmss}"
        };
        IRenderable src = new Mandelbrot(opts.Height, opts.Width, 0.001);

        var renderer = new Renderer(rendersDir);
        renderer.RenderImageToFile(src, opts);
    }


    // Keep this until it's enshrined in a Scene/Renderer, it's just Fbm now
    private static void Main_NoiseVideo(string[] args)
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

        var frames = NoiseFrames(count: 100, width: 1200, height: 800);
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

    private static IEnumerable<IVideoFrame> NoiseFrames(int count, int width, int height)
    {
        var scale = 1 / 1000f;
        for (int t = 0; t < count; t++)
        {
            var frame = new byte[height, width, 3];
            Console.WriteLine($"\rRendering frame {t + 1} of {count}");

            Parallel.For(0, width, i =>
            {
                for (var j = 0; j < height; ++j)
                {
                    var sample = Fbm.Pattern3(new Vector2((float)i * scale, (float)j * scale), t * scale);
                    var rgb = (byte)(sample * 255);
                    frame[j, i, 0] = rgb;
                    frame[j, i, 1] = rgb;
                    frame[j, i, 2] = rgb;
                }
            });
            var bm = Utilities.ArrayToImage(frame);

            using var result = new SKBitmapFrame(bm);
            yield return result;
        }
    }
}
