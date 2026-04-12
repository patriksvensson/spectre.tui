namespace Spectre.Tui;

internal sealed class SparseBuffer : IBuffer
{
    private readonly Dictionary<Position, Cell> _cells = new();

    public IEnumerable<(Position Position, Cell Cell)> Cells => _cells.Select(kvp => (kvp.Key, kvp.Value));

    public Cell? GetCell(int x, int y)
    {
        var position = new Position(x, y);
        if (!_cells.TryGetValue(position, out var cell))
        {
            cell = new Cell();
            _cells[position] = cell;
        }

        return cell;
    }

    public void Clear()
    {
        _cells.Clear();
    }
}
