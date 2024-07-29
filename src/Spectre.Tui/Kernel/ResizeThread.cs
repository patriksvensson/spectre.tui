namespace Spectre.Tui;

internal sealed class ResizeThread : KernelThread
{
    private readonly Driver _driver;

    protected override string Name { get; } = "Resize";

    public ResizeThread(Driver driver, IMessageDispatcher dispatcher)
        : base(dispatcher)
    {
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    protected override void Run()
    {
        var size = new Size(0, 0);

        while (!Stopping.WaitOne(0))
        {
            var newSize = _driver.GetTerminalSize();
            if (newSize.Width != size.Width || newSize.Height != size.Height)
            {
                size = newSize;
                Post(
                    new ResizeEvent
                    {
                        Size = size,
                    });
            }

            if (Stopping.WaitOne(10))
            {
                break;
            }
        }
    }
}

[UnsupportedOSPlatform("windows")]
internal sealed class ResizeSignal : IKernelThread, IDisposable
{
    private readonly Driver _driver;
    private readonly IMessageDispatcher _dispatcher;
    private PosixSignalRegistration? _signal;

    public bool IsRunning { get; private set; }

    public ResizeSignal(Driver driver, IMessageDispatcher dispatcher)
    {
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    public void Dispose()
    {
        _signal?.Dispose();
    }

    public void Start()
    {
        _signal = PosixSignalRegistration.Create(
            PosixSignal.SIGWINCH,
            _ =>
            {
                var size = _driver.GetTerminalSize();
                _dispatcher.DispatchMessage(
                    new ResizeEvent
                    {
                        Size = size,
                    });
            });

        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
        _signal?.Dispose();
        _signal = null;
    }
}