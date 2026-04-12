namespace Spectre.Tui;

internal sealed class SwapChain : IDoubleBuffer
{
    private readonly Buffer[] _buffers;
    private int _bufferIndex;

    public Buffer Front => _buffers[_bufferIndex];
    public Buffer Back => _buffers[1 - _bufferIndex];

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
        return Back.Diff(Front);
    }

    public void Resize(Rectangle screen)
    {
        Front.Resize(screen);
        Back.Resize(screen);

        // Reset the back buffer so next diff is clean
        Back.Reset();
    }

    public void Swap()
    {
        Back.Reset();
        _bufferIndex = 1 - _bufferIndex;
    }
}