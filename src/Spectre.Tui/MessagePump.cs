namespace Spectre.Tui;

[PublicAPI]
public interface IMessagePoster
{
    /// <summary>
    /// Puts a message on the message queue.
    /// </summary>
    /// <param name="message">The message to post.</param>
    void PostMessage(Message message);
}

[PublicAPI]
internal interface IMessageDispatcher
{
    /// <summary>
    /// Dispatches the message immediately, overriding
    /// the message queue.
    /// </summary>
    /// <param name="message">The message to dispatch.</param>
    void DispatchMessage(Message message);
}

[PublicAPI]
public abstract class MessagePump : IMessageDispatcher, IMessagePoster
{
    private readonly ConcurrentQueue<Message> _queue;
    private readonly HashSet<Type> _ignored;
    private MessagePump? _parent;

    protected MessagePump()
    {
        _queue = new ConcurrentQueue<Message>();
        _ignored = new HashSet<Type>();
    }

    public void PostMessage(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (message.Sender == null)
        {
            message.Sender = this;
        }

        _queue.Enqueue(message);
    }

    public void DispatchMessage(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (message.Sender == null)
        {
            message.Sender = this;
        }

        if (!_ignored.Contains(message.GetType()))
        {
            OnMessage(message);
        }

        if (message.Bubble && _parent != null && message.Sender != _parent)
        {
            _parent.DispatchMessage(message);
        }
    }

    protected void IgnoreMessage<T>()
        where T : Message
    {
        _ignored.Add(typeof(T));
    }

    protected abstract void OnMessage(Message message);

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