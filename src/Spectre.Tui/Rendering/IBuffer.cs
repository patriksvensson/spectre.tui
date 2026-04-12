namespace Spectre.Tui;

internal interface IBuffer
{
    Cell? GetCell(int x, int y);
}
