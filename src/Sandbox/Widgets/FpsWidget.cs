namespace Sandbox;

public sealed class FpsWidget : IWidget
{
    private readonly Text _text;

    public FpsWidget(
        double fps,
        Color? foreground = null,
        Color? background = null)
    {
        _text = Text.FromMarkup(
            $"[yellow]FPS:[/] {fps:0.000}",
            new Style
            {
                Foreground = foreground ?? Color.Default,
                Background = background ?? Color.Default,
            });
    }

    public void Render(RenderContext context)
    {
        var width = _text.GetWidth();

        context.Render(
            _text,
            new Rectangle(
                (context.Viewport.Width - width) / 2,
                context.Viewport.Height / 2,
                width, 1));
    }
}