namespace Spectre.Tui.Tests;

public sealed class RenderSurfaceTests
{
    public sealed class Dimensions
    {
        [Fact]
        public void Should_Have_Zero_Width_And_Height_Before_Render()
        {
            // Given, When
            var surface = new RenderSurface();

            // Then
            surface.Width.ShouldBe(0);
            surface.Height.ShouldBe(0);
        }

        [Fact]
        public void Should_Compute_Width_And_Height_From_Written_Cells()
        {
            // Given
            var surface = new RenderSurface();

            // When
            surface.Render(ctx =>
            {
                ctx.SetString(0, 0, "Hello");
                ctx.SetString(0, 1, "Hi");
            });

            // Then
            surface.Width.ShouldBe(5);
            surface.Height.ShouldBe(2);
        }

        [Fact]
        public void Should_Update_Width_And_Height_On_Each_Render()
        {
            // Given
            var surface = new RenderSurface();
            surface.Render(ctx => ctx.SetString(0, 0, "Hello World"));

            // When
            surface.Render(ctx => ctx.SetString(0, 0, "Hi"));

            // Then
            surface.Width.ShouldBe(2);
            surface.Height.ShouldBe(1);
        }
    }

    [Fact]
    public void Should_Clear_Previous_Cells_On_Render()
    {
        // Given
        var surface = new RenderSurface();

        // When
        surface.Render(ctx => ctx.SetString(0, 0, "Hello World"));
        surface.Render(ctx => ctx.SetString(0, 0, "Hi"));

        var fixture = new TuiFixture(new Size(11, 3));
        var result = fixture.Render(frame =>
        {
            frame.Blit(new Position(0, 1), surface);
        });

        // Then
        result.ShouldBe(
            """
            •••••••••••
            Hi•••••••••
            •••••••••••
            """);
    }
}
