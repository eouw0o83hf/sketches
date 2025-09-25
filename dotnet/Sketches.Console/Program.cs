// See https://aka.ms/new-console-template for more information
using SkiaSharp;

using var bm = new SKBitmap(200, 100);
using var canvas = new SKCanvas(bm);

canvas.Clear(SKColors.Red);
for (var i = 0; i < 100; ++i)
{
    canvas.DrawPoint(i * 2, i, SKColors.AliceBlue);
}

using var img = SKImage.FromBitmap(bm);
using var skdata = img.Encode(SKEncodedImageFormat.Png, 100);

var workingDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
var path = Path.Combine(workingDir, "renders/tmp.png");
using var fs = File.OpenWrite(path);
skdata.SaveTo(fs);
