namespace Spectre.Tui;

public abstract class App : MessagePump
{
    private readonly LayoutView _view;
    private readonly List<Task> _children;
    private CancellationToken? _cancellationToken;

    public View View => _view;

    protected App()
    {
        _view = new LayoutView();
        _view.SetParent(this);
        _children = new List<Task>();
    }

    public async Task Run()
    {
        var signal = new CancellationTokenSource();
        System.Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            signal.Cancel();
        };

        await Run(signal.Token);
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        DispatchMessage(new StartedEvent());
        PostMessage(new MountedEvent());

        var driver = Driver.Create();
        var kernel = new Kernel(driver, this);
        using (kernel.Start())
        {
            // Register the view
            RegisterChild(_view);

            // Start and await the message pump
            await Start(cancellationToken);

            // Wait for all children to exit
            Task.WaitAll(_children.ToArray(), cancellationToken: default);
        }
    }

    protected override void OnMessage(Message message)
    {
        if (message is StartedEvent)
        {
            OnStarted();
        }

        // Forward the message to the view
        _view.ForwardMessage(message);
    }

    /// <summary>
    /// Called when the application has been started.
    /// This is where you mount widgets for your application.
    /// </summary>
    protected virtual void OnStarted()
    {
    }

    internal void RegisterChild(MessagePump child)
    {
        if (_cancellationToken == null)
        {
            throw new InvalidOperationException("Application has not been started");
        }

        child.SetParent(this);
        _children.Add(child.Start(_cancellationToken.Value));
    }
}