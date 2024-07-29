namespace Spectre.Tui;

[PublicAPI]
public interface IMessage
{
    public bool Bubble => false;
    public object? Sender => null;
}

[PublicAPI]
public struct ResizeEvent : IMessage
{
    public required Size Size { get; init; }
}

[PublicAPI]
public struct KeyDownEvent : IMessage
{
    public required ConsoleKeyInfo Key { get; init; }
}