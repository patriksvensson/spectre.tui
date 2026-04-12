namespace Spectre.Tui;

public sealed class Border
{
    public required char TopLeft { get; init; }
    public required char TopRight { get; init; }
    public required char BottomLeft { get; init; }
    public required char BottomRight { get; init; }
    public required char VerticalLeft { get; init; }
    public required char VerticalRight { get; init; }
    public required char HorizontalTop { get; init; }
    public required char HorizontalBottom { get; init; }

    public Border()
    {
    }

    [SetsRequiredMembers]
    public Border(Line line)
    {
        TopLeft = line.TopLeft;
        TopRight = line.TopRight;
        BottomLeft = line.BottomLeft;
        BottomRight = line.BottomRight;
        VerticalLeft = line.Vertical;
        VerticalRight = line.Vertical;
        HorizontalTop = line.Horizontal;
        HorizontalBottom = line.Horizontal;
    }

    public static Border Plain { get; } = new(Line.Plain);
    public static Border Rounded { get; } = new(Line.Rounded);
    public static Border Double { get; } = new(Line.Double);
    public static Border Bold { get; } = new(Line.Bold);

    public static Border McGuganWide { get; } = new()
    {
        TopLeft = Line.Symbols.OneEightBottom,
        TopRight = Line.Symbols.OneEightBottom,
        BottomLeft = Line.Symbols.OneEightTop,
        BottomRight = Line.Symbols.OneEightTop,
        VerticalLeft = Line.Symbols.OneEightLeft,
        VerticalRight = Line.Symbols.OneEightRight,
        HorizontalTop = Line.Symbols.OneEightBottom,
        HorizontalBottom = Line.Symbols.OneEightTop,
    };

    public static Border McGuganTall { get; } = new()
    {
        TopLeft = Line.Symbols.OneEightRight,
        TopRight = Line.Symbols.OneEightLeft,
        BottomLeft = Line.Symbols.OneEightRight,
        BottomRight = Line.Symbols.OneEightLeft,
        VerticalLeft = Line.Symbols.OneEightRight,
        VerticalRight = Line.Symbols.OneEightLeft,
        HorizontalTop = Line.Symbols.OneEightTop,
        HorizontalBottom = Line.Symbols.OneEightBottom,
    };
}
