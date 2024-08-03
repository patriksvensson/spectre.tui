namespace Spectre.Tui;

internal sealed class Kernel
{
    private readonly List<IKernelWorker> _threads;

    public Kernel(Driver driver, IMessageDispatcher dispatcher)
    {
        _threads = new List<IKernelWorker>();
        _threads.Add(new KeyboardThread(driver, dispatcher));
        _threads.AddRange(driver.GetWorker(dispatcher));
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
            AnsiConsole.MarkupLine($"[green]Starting:[/] {thread.Name}");
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
            AnsiConsole.MarkupLine($"[green]Stopping:[/] {thread.Name}");
            thread.Stop();
        }
    }
}