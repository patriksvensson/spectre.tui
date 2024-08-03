namespace Spectre.Tui;

internal sealed class ResizeThread : KernelThread
{
    private readonly Driver _driver;

    public override string Name { get; } = "Resize Thread";

    public ResizeThread(Driver driver, IMessageDispatcher dispatcher)
        : base(dispatcher)
    {
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    protected override void Run()
    {
        var size = _driver.GetTerminalSize();

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