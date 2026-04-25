namespace Spectre.Tui.Tests;

public sealed class RendererTests
{
    private sealed class TestTextWidget(int x, int y, string text) : IWidget
    {
        public void Render(RenderContext context)
        {
            for (var i = 0; i < text.Length; i++)
            {
                context.SetSymbol(x + i, y, text[i]);
            }
        }
    }

    [Fact]
    public void Should_Render_Buffer()
    {
        // Given
        var fixture = new TuiFixture(
            new Size(11, 5));

        // When
        var result = fixture.Render(frame =>
        {
            frame.Render(new TestTextWidget(3, 2, "Hello"));
        });

        // Then
        result.ShouldBe(
            """
            •••••••••••
            •••••••••••
            •••Hello•••
            •••••••••••
            •••••••••••
            """);
    }

    [Fact]
    public void Should_Only_Render_Diff_Between_Frames()
    {
        // Given
        var fixture = new TuiFixture(new Size(11, 5));
        fixture.Render(new TestTextWidget(3, 1, "Hello"));

        // When
        var result = fixture.Render(frame =>
        {
            frame.Render(new TestTextWidget(3, 1, "Hello"));
            frame.Render(new TestTextWidget(3, 2, "World"));
        });

        // Then
        result.ShouldBe(
            """
            •••••••••••
            •••••••••••
            •••World•••
            •••••••••••
            •••••••••••
            """);
    }

    [Fact]
    public void Should_Blit_Surface_At_Given_Position()
    {
        // Given
        var fixture = new TuiFixture(new Size(11, 3));
        var surface = new RenderSurface();
        surface.Render(ctx =>
        {
            ctx.Render(new TestTextWidget(0, 0, "Hello"));
        });

        // When
        var result = fixture.Render(frame =>
        {
            frame.Blit(new Position(3, 1), surface);
        });

        // Then
        result.ShouldBe(
            """
            •••••••••••
            •••Hello•••
            •••••••••••
            """);
    }

    [Fact]
    public void Should_Blit_Source_Region_Of_Surface()
    {
        // Given
        var fixture = new TuiFixture(new Size(11, 3));
        var surface = new RenderSurface();
        surface.Render(ctx =>
        {
            ctx.Render(new TestTextWidget(0, 0, "Hello World"));
        });

        // When
        var result = fixture.Render(frame =>
        {
            frame.Blit(3, 1, surface, new Rectangle(6, 0, 5, 1));
        });

        // Then
        result.ShouldBe(
            """
            •••••••••••
            •••World•••
            •••••••••••
            """);
    }

    [Fact]
    public void Should_Blit_Source_Region_With_Position_Overload()
    {
        // Given
        var fixture = new TuiFixture(new Size(11, 3));
        var surface = new RenderSurface();
        surface.Render(ctx =>
        {
            ctx.Render(new TestTextWidget(0, 0, "Hello"));
            ctx.Render(new TestTextWidget(0, 1, "World"));
        });

        // When
        var result = fixture.Render(frame =>
        {
            frame.Blit(new Position(3, 1), surface, new Rectangle(0, 1, 5, 1));
        });

        // Then
        result.ShouldBe(
            """
            •••••••••••
            •••World•••
            •••••••••••
            """);
    }

    [Fact]
    public void Should_Skip_Cells_Outside_Source_Region()
    {
        // Given
        var fixture = new TuiFixture(new Size(11, 3));
        var surface = new RenderSurface();
        surface.Render(ctx =>
        {
            ctx.Render(new TestTextWidget(0, 0, "Hello"));
            ctx.Render(new TestTextWidget(0, 1, "World"));
        });

        // When
        var result = fixture.Render(frame =>
        {
            frame.Blit(3, 1, surface, new Rectangle(1, 0, 3, 1));
        });

        // Then
        result.ShouldBe(
            """
            •••••••••••
            •••ell•••••
            •••••••••••
            """);
    }

    [Fact]
    public void Should_No_Op_When_Source_Region_Is_Empty()
    {
        // Given
        var fixture = new TuiFixture(new Size(11, 3));
        var surface = new RenderSurface();
        surface.Render(ctx =>
        {
            ctx.Render(new TestTextWidget(0, 0, "Hello"));
        });

        // When
        var result = fixture.Render(frame =>
        {
            frame.Blit(3, 1, surface, new Rectangle(0, 0, 0, 0));
        });

        // Then
        result.ShouldBe(
            """
            •••••••••••
            •••••••••••
            •••••••••••
            """);
    }
}