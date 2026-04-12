namespace Spectre.Tui;

internal sealed class SwapChain
{
    private readonly Buffer[] _buffers;
    private int _bufferIndex;

    public Buffer Current => _buffers[_bufferIndex];
    public Buffer Previous => _buffers[1 - _bufferIndex];

    public SwapChain(Rectangle size)
    {
        _buffers =
        [
            Buffer.Empty(size),
            Buffer.Empty(size),
        ];
    }

    public IEnumerable<(int x, int y, Cell)> Diff()
    {
        return Previous.Diff(Current);
    }

    public void Resize(Rectangle screen)
    {
        Current.Resize(screen);
        Previous.Resize(screen);

        // Reset the back buffer
        Previous.Reset();
    }

    public void Swap()
    {
        Previous.Reset();
        _bufferIndex = 1 - _bufferIndex;
    }
}