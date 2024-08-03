namespace Spectre.Tui;

public abstract class Message
{
    public object? Sender { get; internal set; }
    public virtual bool Bubble => false;
}

[PublicAPI]
public sealed class ResizeEvent : Message
{
    public required Size Size { get; init; }
}

[PublicAPI]
public sealed class KeyDownEvent : Message
{
    public required ConsoleKeyInfo Key { get; init; }
}

[PublicAPI]
public sealed class StartedEvent : Message
{
}

[PublicAPI]
public sealed class MountedEvent : Message
{
}