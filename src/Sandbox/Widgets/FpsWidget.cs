namespace Sandbox;

public sealed class FpsWidget : IWidget
{
    private readonly Paragraph _paragraph;

    public FpsWidget(
        double fps,
        Color? foreground = null,
        Color? background = null)
    {
        _paragraph = Paragraph.FromMarkup(
            $"[yellow]FPS:[/] {fps:0.000}",
            new Style
            {
                Foreground = foreground ?? Color.Default,
                Background = background ?? Color.Default,
            }).WithAlignment(Justify.Center);
    }

    public void Render(RenderContext context)
    {
        context.Render(_paragraph);
    }
}