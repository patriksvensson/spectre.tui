namespace Spectre.Tui;

/// <summary>
/// View represent the root view that composes the
/// rest of the user interface and transmits events
/// and messages to it's widgets.
/// </summary>
[PublicAPI]
public abstract class View : MessagePump
{
    // Doesn't really do anything at this point
    public abstract void Mount();

    internal void ForwardMessage(Message message)
    {
        DispatchMessage(message);
    }
}

internal sealed class LayoutView : View
{
    public LayoutView()
    {
        IgnoreMessage<StartedEvent>();
    }

    protected override void OnMessage(Message message)
    {
        if (message is KeyDownEvent keyDown)
        {
            AnsiConsole.MarkupLine($"[yellow]KeyDown:[/] {keyDown.Key.KeyChar}");
        }
        else if (message is ResizeEvent resize)
        {
            AnsiConsole.MarkupLine($"[yellow]Resize:[/] {resize.Size.Width}, {resize.Size.Height}");
        }
        else
        {
            AnsiConsole.MarkupLine($"[yellow]Event:[/] {message.GetType().Name}");
        }
    }

    public override void Mount()
    {
    }
}