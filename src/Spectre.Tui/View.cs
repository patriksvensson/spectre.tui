namespace Spectre.Tui;

/// <summary>
/// View represent the root view that composes the
/// rest of the user interface and transmits events
/// and messages to it's widgets.
/// </summary>
[PublicAPI]
public abstract class View : MessagePump
{
    internal void ForwardMessage(IMessage message)
    {
        OnMessage(message);
    }
}

internal sealed class LayoutView : View
{
    protected override void OnMessage(IMessage message)
    {
        if (message is KeyDownEvent keyDown)
        {
            AnsiConsole.WriteLine($"KeyDown: {keyDown.Key.KeyChar}");
        }
        else if (message is ResizeEvent resize)
        {
            AnsiConsole.WriteLine($"Resize: {resize.Size.Width}, {resize.Size.Height}");
        }
    }
}