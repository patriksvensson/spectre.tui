namespace Spectre.Tui;

public abstract class JustInTimeWidget : IWidget
{
    private readonly RenderSurface _surface = new();
    private bool _dirty = true;
    private Rectangle? _viewport;

    protected void MarkAsDirty()
    {
        _dirty = true;
    }

    void IWidget.Render(RenderContext context)
    {
        if (_viewport == null || _viewport.Value != context.Viewport)
        {
            _dirty = true;
        }

        _viewport = context.Viewport;

        if (_dirty)
        {
            _surface.Render(RenderDirty);
            _dirty = false;
        }

        context.Blit(0, 0, _surface);
    }

    protected abstract void RenderDirty(RenderContext context);
}