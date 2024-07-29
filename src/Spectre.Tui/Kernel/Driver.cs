namespace Spectre.Tui;

public abstract class Driver
{
    public abstract Size GetTerminalSize();
    public abstract ConsoleKeyInfo? GetPressedKey();
}

public sealed class DefaultDriver : Driver
{
    public override Size GetTerminalSize()
    {
        return new Size(
            System.Console.WindowWidth,
            System.Console.WindowHeight);
    }

    public override ConsoleKeyInfo? GetPressedKey()
    {
        if (System.Console.KeyAvailable)
        {
            return System.Console.ReadKey(true);
        }

        return null;
    }
}