namespace Spectre.Tui;

[PublicAPI]
public sealed class RenderSurface
{
    private readonly SparseBuffer _buffer = new();

    public int Width { get; private set; }
    public int Height { get; private set; }

    public void Render(Action<RenderContext> render)
    {
        _buffer.Clear();

        render(new RenderContext(_buffer, new Rectangle(0, 0, short.MaxValue, short.MaxValue)));

        var width = 0;
        var height = 0;
        foreach (var (position, _) in _buffer.Cells)
        {
            if (position.X + 1 > width)
            {
                width = position.X + 1;
            }

            if (position.Y + 1 > height)
            {
                height = position.Y + 1;
            }
        }

        Width = width;
        Height = height;
    }

    internal IEnumerable<(Position Position, Cell Cell)> GetCells()
    {
        return _buffer.Cells;
    }
}