namespace Spectre.Tui;

internal sealed class Kernel
{
    private readonly List<IKernelThread> _threads;

    public Kernel(Driver driver, IMessageDispatcher dispatcher)
    {
        _threads = new List<IKernelThread>();
        _threads.Add(new KeyboardThread(driver, dispatcher));

        if (!OperatingSystem.IsWindows() && !Constants.IsDebug)
        {
            _threads.Add(new ResizeSignal(driver, dispatcher));
        }
        else
        {
            _threads.Add(new ResizeThread(driver, dispatcher));
        }
    }

    private sealed class Scope : IDisposable
    {
        private readonly Kernel _kernel;

        public Scope(Kernel kernel)
        {
            _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        }

        public void Dispose()
        {
            _kernel.Stop();
        }
    }

    public IDisposable Start()
    {
        foreach (var thread in _threads)
        {
            thread.Start();
        }

        while (true)
        {
            if (_threads.All(x => x.IsRunning))
            {
                break;
            }
        }

        return new Scope(this);
    }

    private void Stop()
    {
        foreach (var thread in _threads)
        {
            thread.Stop();
        }
    }
}