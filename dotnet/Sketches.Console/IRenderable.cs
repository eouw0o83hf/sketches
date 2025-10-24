namespace Sketches.Console;

public interface IRenderable
{
    /// <summary>
    /// [y * width + x] -> 1D array of a frame, row by row
    /// </summary>
    uint[] Render(float t);
}
