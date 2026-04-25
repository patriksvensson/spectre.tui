namespace Spectre.Tui;

[PublicAPI]
public sealed class BoxWidget(Style? style = null) : IWidget
{
    public Style? Style { get; set; }
    public Border Border { get; set; } = Border.Rounded;
    public IWidget? Inner { get; set; }

    public void Render(RenderContext context)
    {
        var area = context.Viewport;

        if (area.Height == 0)
        {
            return;
        }

        if (area.Height == 1)
        {
            for (var x = 0; x < area.Width; x++)
            {
                if (x == 0)
                {
                    context.SetSymbol(x, 0, Border.VerticalLeft);
                }
                else if (x == area.Width - 1)
                {
                    context.SetSymbol(x, 0, Border.VerticalRight);
                }
                else
                {
                    context.SetSymbol(x, 0, Border.HorizontalTop);
                }

                context.SetStyle(x, 0, style);
            }

            return;
        }

        // Top/Bottom
        for (var x = 0; x < area.Width; x++)
        {
            if (x == 0)
            {
                context.SetSymbol(x, 0, Border.TopLeft);
                context.SetStyle(x, 0, style);
                context.SetSymbol(x, area.Height - 1, Border.BottomLeft);
                context.SetStyle(x, area.Height - 1, style);
            }
            else if (x == area.Width - 1)
            {
                context.SetSymbol(x, 0, Border.TopRight);
                context.SetStyle(x, 0, style);
                context.SetSymbol(x, area.Height - 1, Border.BottomRight);
                context.SetStyle(x, area.Height - 1, style);
            }
            else
            {
                context.SetSymbol(x, 0, Border.HorizontalTop);
                context.SetStyle(x, 0, style);
                context.SetSymbol(x, area.Height - 1, Border.HorizontalBottom);
                context.SetStyle(x, area.Height - 1, style);
            }
        }

        // Sides
        for (var y = 1; y < area.Height - 1; y++)
        {
            context.SetSymbol(0, y, Border.VerticalLeft);
            context.SetStyle(0, y, style);
            context.SetSymbol(area.Width - 1, y, Border.VerticalRight);
            context.SetStyle(area.Width - 1, y, style);
        }

        if (Inner != null)
        {
            context.Render(Inner, context.Viewport.Inflate(-1, -1));
        }
    }
}

[PublicAPI]
public static class BoxWidgetExtensions
{
    extension(BoxWidget widget)
    {
        public BoxWidget WithStyle(Style? style)
        {
            widget.Style = style;
            return widget;
        }

        public BoxWidget WithBorder(Border border)
        {
            widget.Border = border;
            return widget;
        }

        public BoxWidget WithInnerWidget(IWidget inner)
        {
            widget.Inner = inner;
            return widget;
        }
    }
}