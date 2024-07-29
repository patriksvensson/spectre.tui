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
}