namespace Spectre.Tui;

[PublicAPI]
public interface IMessagePoster
{
    /// <summary>
    /// Puts a message on the message queue.
    /// </summary>
    /// <param name="message">The message to post.</param>
    void PostMessage(IMessage message);
}

[PublicAPI]
internal interface IMessageDispatcher
{
    /// <summary>
    /// Dispatches the message immediately, overriding
    /// the message queue.
    /// </summary>
    /// <param name="message">The message to dispatch.</param>
    void DispatchMessage(IMessage message);
}

[PublicAPI]
public abstract class MessagePump : IMessageDispatcher, IMessagePoster
{
    private readonly ConcurrentQueue<IMessage> _queue;
    private MessagePump? _parent;

    protected MessagePump()
    {
        _queue = new ConcurrentQueue<IMessage>();
    }

    public void PostMessage(IMessage message)
    {
        _queue.Enqueue(message);
    }

    public void DispatchMessage(IMessage message)
    {
        OnMessage(message);

        if (message.Bubble && _parent != null && message.Sender != _parent)
        {
            _parent.DispatchMessage(message);
        }
    }

    protected abstract void OnMessage(IMessage message);

    internal void SetParent(MessagePump? parent)
    {
        _parent = parent;
    }

    internal Task Start(CancellationToken cancellationToken = default)
    {
        return Task.Run(
            () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_queue.TryDequeue(out var message))
                    {
                        DispatchMessage(message);
                    }
                }
            }, cancellationToken);
    }
}