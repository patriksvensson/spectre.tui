namespace Spectre.Tui;

[PublicAPI]
public sealed record RenderContext
{
    private readonly IDoubleBuffer _buffer;

    public Rectangle Screen { get; internal init; }
    public Rectangle Viewport { get; internal init; }

    internal RenderContext? Parent { get; init; }
    internal Position? CursorPosition
    {
        get
        {
            return Parent != null ? Parent.CursorPosition : field;
        }
        set
        {
            if (Parent != null)
            {
                Parent.CursorPosition = value;
                return;
            }

            field = value;
        }
    }

    internal RenderContext(IDoubleBuffer buffer, Rectangle screen)
    {
        _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));

        Parent = null;
        Screen = screen;
        Viewport = screen;
    }

    public Cell? GetCell(int x, int y)
    {
        return _buffer.Front.GetCell(Screen.X + x, Screen.Y + y);
    }

    public IReadOnlyCell? GetCellFromPreviousFrame(int x, int y)
    {
        return _buffer.Back.GetCell(Screen.X + x, Screen.Y + y);
    }
}

[PublicAPI]
public static class RenderContextExtensions
{
    extension(RenderContext context)
    {
        public void Render(IWidget widget)
        {
            widget.Render(context);
        }

        public void Render(IWidget widget, Rectangle area)
        {
            if (context.CreateAreaRenderContext(area, out var areaContext))
            {
                widget.Render(areaContext);
            }
        }

        public void Render<TState>(IStatefulWidget<TState> widget, TState state)
        {
            widget.Render(context, state);
        }

        public void Render<TState>(IStatefulWidget<TState> widget, Rectangle area, TState state)
        {
            if (context.CreateAreaRenderContext(area, out var areaContext))
            {
                widget.Render(areaContext, state);
            }
        }

        public Position SetString(int x, int y, string text, Style? style = null, int? maxWidth = null)
        {
            var remainingWidth = Math.Min(
                context.Viewport.Right - x,
                maxWidth ?? context.Viewport.Right);

            foreach (var symbol in text.Graphemes())
            {
                var width = symbol.GetCellWidth();

                // Skip zero-width characters
                if (width == 0)
                {
                    continue;
                }

                // Stop when no more horizontal space is available
                remainingWidth -= width;
                if (remainingWidth < 0)
                {
                    break;
                }

                context.GetCell(x, y)?
                    .SetSymbol(symbol)
                    .SetStyle(style);

                var nextSymbolPosition = x + width;
                x++;

                // Reset the next following cells
                while (x < nextSymbolPosition)
                {
                    context.GetCell(x, y)?.SetSymbol("");
                    x++;
                }
            }

            return new Position(x, y);
        }

        public Position SetLine(int x, int y, TextLine line, int maxWidth)
        {
            var remainingWidth = maxWidth;
            foreach (var span in line.Spans)
            {
                if (remainingWidth == 0)
                {
                    break;
                }

                var pos = context.SetString(
                    x, y,
                    span.Text,
                    line.Style?.Combine(span.Style) ?? span.Style,
                    remainingWidth);

                var w = pos.X - x;
                x = pos.X;
                remainingWidth -= w;
            }

            return new Position(x, y);
        }

        public Position SetSpan(int x, int y, TextSpan span, int maxWidth)
        {
            return context.SetString(x, y, span.Text, span.Style, maxWidth);
        }

        public void SetSymbol(int x, int y, char symbol)
        {
            context.GetCell(x, y)?.SetSymbol(symbol);
        }

        public void SetSymbol(int x, int y, Rune symbol)
        {
            context.GetCell(x, y)?.SetSymbol(symbol);
        }

        public void SetStyle(int x, int y, Style? style)
        {
            context.GetCell(x, y)?.SetStyle(style);
        }

        public void SetStyle(Rectangle area, Style? style)
        {
            if (style == null)
            {
                return;
            }

            var intersected = context.Viewport.Intersect(area);
            for (var y = intersected.Y; y < intersected.Y + intersected.Height; y++)
            {
                for (var x = intersected.X; x < intersected.X + intersected.Width; x++)
                {
                    context.GetCell(x, y)?.SetStyle(style);
                }
            }
        }

        public void SetForeground(int x, int y, Color? color)
        {
            context.GetCell(x, y)?.SetForeground(color);
        }

        public void SetBackground(int x, int y, Color? color)
        {
            context.GetCell(x, y)?.SetBackground(color);
        }

        public void SetCursorPosition(Position position)
        {
            context.CursorPosition = position;
        }

        private bool CreateAreaRenderContext(Rectangle area, [NotNullWhen(true)] out RenderContext? result)
        {
            if (area.IsEmpty)
            {
                result = null;
                return false;
            }

            var screen = context.Screen.Intersect(
                new Rectangle(
                    context.Screen.X + area.X, context.Screen.Y + area.Y,
                    area.Width, area.Height));

            // The provided screen does not intersect with
            // the area. We can't render anything in this rectangle.
            if (screen.IsEmpty)
            {
                result = null;
                return false;
            }

            result = context with
            {
                Parent = context,
                Screen = screen,
                Viewport = new Rectangle(0, 0, screen.Width, screen.Height)
            };

            return true;
        }
    }
}