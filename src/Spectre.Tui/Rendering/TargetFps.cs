namespace Spectre.Tui;

internal sealed class TargetFps
{
    public TimeSpan Accumulated { get; private set; }

    public TimeSpan? Target
    {
        get;
        set
        {
            Accumulated = TimeSpan.Zero;
            field = value;
        }
    }

    public bool ShouldRedraw(TimeSpan delta)
    {
        if (Target == null)
        {
            // Uncapped FPS
            return true;
        }

        Accumulated += delta;

        if (Accumulated >= Target)
        {
            Accumulated = TimeSpan.Zero;
            return true;
        }

        return false;
    }
}