namespace Spectre.Tui;

internal interface IKernelThread
{
    bool IsRunning { get; }
    void Start();
    void Stop();
}

internal abstract class KernelThread : IKernelThread
{
    private readonly IMessageDispatcher _dispatcher;
    private readonly Thread _thread;
    private readonly ManualResetEvent _running;
    private readonly ManualResetEvent _stopping;

    public bool IsRunning => _running.WaitOne(0);

    protected WaitHandle Stopping => _stopping;
    protected abstract string Name { get; }

    protected KernelThread(IMessageDispatcher dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _running = new ManualResetEvent(false);
        _stopping = new ManualResetEvent(false);
        _thread = new Thread(Run);
    }

    public void Start()
    {
        if (_running.WaitOne(0))
        {
            return;
        }

        _running.Set();
        _thread.Start();
    }

    public void Stop()
    {
        if (!_running.WaitOne(0))
        {
            return;
        }

        // Signal threads to stop
        Debug.WriteLine($"Signaling thread '{Name}' to stop");
        _stopping.Set();
        _thread.Join();
        Debug.WriteLine($"Thread '{Name}' has stopped");

        // Reset signals
        _running.Reset();
        _stopping.Reset();
    }

    protected abstract void Run();

    protected void Post(IMessage message)
    {
        _dispatcher.DispatchMessage(message);
    }
}