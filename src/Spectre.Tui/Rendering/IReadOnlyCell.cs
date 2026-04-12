namespace Spectre.Tui;

public interface IReadOnlyCell
{
    string Symbol { get; }
    Style Style { get; }

    Decoration Decoration { get; }
    Color Foreground { get; }
    Color Background { get; }
}