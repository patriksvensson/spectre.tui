using Spectre.Console;
using Spectre.Tui;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var app = new MyApp();
        await app.Run();
    }
}

public sealed class MyApp : App
{
    protected override void OnStarted()
    {
        AnsiConsole.MarkupLine("[blue]App started[/]");

        // Pretend to mount some widgets, just
        // to test out the message propagation.
        View.Mount();
    }
}