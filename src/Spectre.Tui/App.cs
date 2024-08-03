namespace Spectre.Tui;

public abstract class App : MessagePump
{
    private readonly View _view;

    protected App()
    {
        _view = new LayoutView();
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
        var driver = Driver.Create();
        var kernel = new Kernel(driver, this);
        using (kernel.Start())
        {
            // Start and await the message pump
            await Start(cancellationToken);
        }
    }

    protected override void OnMessage(IMessage message)
    {
        // Forward the message to the view
        _view.ForwardMessage(message);
    }
}