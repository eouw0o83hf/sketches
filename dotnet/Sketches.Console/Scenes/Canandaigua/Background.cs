namespace Sketches.Console.Scenes.Canandaigua;

public class Background(int _height, int _width) : IRenderable
{
    private static readonly uint _color = 0x6666ccff;

    public uint[] Render(float t)
        => Enumerable.Range(0, _width * _height)
            .Select(a => _color)
            .ToArray();
}
