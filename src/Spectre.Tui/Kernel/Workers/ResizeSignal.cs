namespace Spectre.Tui;

[UnsupportedOSPlatform("windows")]
internal sealed class ResizeSignal : IKernelWorker, IDisposable
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