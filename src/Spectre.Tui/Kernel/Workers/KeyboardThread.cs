namespace Spectre.Tui;

internal class KeyboardThread : KernelThread
{
    private readonly Driver _driver;

    public override string Name { get; } = "Keyboard Thread";

    public KeyboardThread(Driver driver, IMessageDispatcher dispatcher)
        : base(dispatcher)
    {
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    protected override void Run()
    {
        while (!Stopping.WaitOne(0))
        {
            var key = _driver.GetPressedKey();
            if (key != null)
            {
                Post(
                    new KeyDownEvent
                    {
                        Key = key.Value,
                    });
            }

            if (Stopping.WaitOne(10))
            {
                break;
            }
        }
    }
}