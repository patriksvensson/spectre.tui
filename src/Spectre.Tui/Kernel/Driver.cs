namespace Spectre.Tui;

internal class Driver
{
    public virtual IEnumerable<IKernelWorker> GetWorker(IMessageDispatcher dispatcher)
    {
        yield return new ResizeThread(this, dispatcher);
    }

    public virtual Size GetTerminalSize()
    {
        return new Size(
            System.Console.WindowWidth,
            System.Console.WindowHeight);
    }

    public virtual ConsoleKeyInfo? GetPressedKey()
    {
        if (System.Console.KeyAvailable)
        {
            return System.Console.ReadKey(true);
        }

        return null;
    }

    public static Driver Create()
    {
        if (OperatingSystem.IsWindows())
        {
            return new Driver();
        }
        else
        {
            return new LinuxDriver();
        }
    }
}

[UnsupportedOSPlatform("windows")]
internal sealed class LinuxDriver : Driver
{
    public override IEnumerable<IKernelWorker> GetWorker(IMessageDispatcher dispatcher)
    {
#if RELEASE
        yield return new ResizeSignal(this, dispatcher);
#else
        return base.GetWorker(dispatcher);
#endif
    }
}