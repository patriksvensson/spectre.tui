# `Spectre.Tui`

_[![Spectre.TUI NuGet Version](https://img.shields.io/nuget/v/spectre.tui.svg?style=flat&label=NuGet%3A%20Spectre.Tui)](https://www.nuget.org/packages/spectre.tui)_

> [!WARNING]
> This library is currently under construction and may change at any time.  
> Use it only if you’re comfortable with this.

## Core library

This example uses only the core `Spectre.Tui` library to render a 
widget on the screen. Creation of the renderer, render loop, and input 
handling is up to the user. Use this if you want to build your own
application model.

```csharp
using Spectre.Tui;

using var terminal = Terminal.Create();
var renderer = new Renderer(terminal);

while (true)
{
    renderer.Draw(async (ctx, _) =>
    {
        ctx.Render(
            Paragraph.FromMarkup(
                """
                [yellow]Hello[/] World!
                Press [bold]Q[/] to quit
                """)
                .Centered()
                .VerticalAlignment(VerticalAlignment.Middle));
    });

    // Handle input
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(true).Key;
        switch (key)
        {
            case ConsoleKey.Q:
                return;
        }
    }
}
```

## Application library

The `Spectre.Tui.App` library is a higher level library for 
writing TUI apps. It contains input handling, a render loop, update loop, and
a screen stack, that can be used to push popups or new screens above the current one.

```csharp
using Spectre.Tui;
using Spectre.Tui.App;

await Application.Create()
    .RunAsync(new MainScreen());

public class MainScreen : Screen
{
    public override void OnMessage(ApplicationContext context, ApplicationMessage message)
    {
        if (message is KeyMessage key && key.Info.Key == ConsoleKey.Spacebar)
        {
            context.Push(new PopUp());
        }
    }

    public override void Render(RenderContext context, FrameInfo frame)
    {
        context.Render(
            Paragraph.FromMarkup(
                """
                Press [yellow]SPACE[/] to open
                Press [blue]CTRL+C[/] to quit the application
                """
            )
                .Centered()
                .AlignedMiddle()
        );
    }
}

public class PopUp : Screen
{
    public override bool IsTransparent => true;

    public override void OnMessage(ApplicationContext context, ApplicationMessage message)
    {
        if (message is KeyMessage key && key.Info.Key == ConsoleKey.Escape)
        {
            context.Pop();
        }
    }

    public override void Render(RenderContext context, FrameInfo frame)
    {
        context.Render(
            new PopupWidget(new Size(50, 10))
                .Content(
                    new BoxWidget()
                        .Border(Border.Rounded)
                        .Title("Popup", TitlePosition.Top, Justify.Center)
                        .Inner(Paragraph.FromMarkup("Press [yellow]ESC[/] to close"))
                ));
    }
}
```

## License

Copyright © Patrik Svensson and contributors

`Spectre.Tui` is provided as-is under the MIT license. For more information see LICENSE.
