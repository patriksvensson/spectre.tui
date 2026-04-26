using Spectre.Console;

namespace Spectre.Tui.Tests;

public sealed class BoxWidgetTests
{
    public sealed class TheTitleMethod
    {
        [Fact]
        public void Should_Default_To_Top_Left()
        {
            // Given
            var box = new BoxWidget().Title("Hello");

            // Then
            box.Titles.Count.ShouldBe(1);
            box.Titles[0].Position.ShouldBe(TitlePosition.Top);
            box.Titles[0].Alignment.ShouldBe(Justify.Left);
        }

        [Fact]
        public void Should_Render_Top_Left_Title()
        {
            // Given
            var fixture = new TuiFixture(new Size(12, 3));

            // When
            var result = fixture.Render(
                new BoxWidget().Title("Hi"));

            // Then
            result.ShouldBe(
                """
                ╭Hi────────╮
                │••••••••••│
                ╰──────────╯
                """);
        }

        [Fact]
        public void Should_Render_Top_Center_Title()
        {
            // Given
            var fixture = new TuiFixture(new Size(12, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .Title("Hi", TitlePosition.Top, Justify.Center));

            // Then
            result.ShouldBe(
                """
                ╭────Hi────╮
                │••••••••••│
                ╰──────────╯
                """);
        }

        [Fact]
        public void Should_Render_Top_Right_Title()
        {
            // Given
            var fixture = new TuiFixture(new Size(12, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .Title("Hi", TitlePosition.Top, Justify.Right));

            // Then
            result.ShouldBe(
                """
                ╭────────Hi╮
                │••••••••••│
                ╰──────────╯
                """);
        }

        [Fact]
        public void Should_Render_Bottom_Center_Title()
        {
            // Given
            var fixture = new TuiFixture(new Size(12, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .Title("Foo", TitlePosition.Bottom, Justify.Center));

            // Then
            result.ShouldBe(
                """
                ╭──────────╮
                │••••••••••│
                ╰───Foo────╯
                """);
        }

        [Fact]
        public void Should_Stack_Titles_With_Space_Separator()
        {
            // Given
            var fixture = new TuiFixture(new Size(14, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .Title("A")
                    .Title("B"));

            // Then
            result.ShouldBe(
                """
                ╭A•B─────────╮
                │••••••••••••│
                ╰────────────╯
                """);
        }

        [Fact]
        public void Should_Render_Mixed_Positions_And_Alignments()
        {
            // Given
            var fixture = new TuiFixture(new Size(14, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .Title("L", TitlePosition.Top, Justify.Left)
                    .Title("R", TitlePosition.Top, Justify.Right)
                    .Title("B", TitlePosition.Bottom, Justify.Center));

            // Then
            result.ShouldBe(
                """
                ╭L──────────R╮
                │••••••••••••│
                ╰─────B──────╯
                """);
        }

        [Fact]
        public void Should_Clip_Title_Wider_Than_Border()
        {
            // Given
            var fixture = new TuiFixture(new Size(8, 3));

            // When
            var result = fixture.Render(
                new BoxWidget().Title("TooLongTitle"));

            // Then
            result.ShouldBe(
                """
                ╭TooLon╮
                │••••••│
                ╰──────╯
                """);
        }

        [Fact]
        public void Should_Render_Title_When_Height_Is_One()
        {
            // Given
            var fixture = new TuiFixture(new Size(7, 1));

            // When
            var result = fixture.Render(
                new BoxWidget().Title("Hi"));

            // Then
            result.ShouldBe("│Hi───│");
        }

        [Fact]
        public void Should_Skip_Titles_When_Width_Has_No_Room_Between_Corners()
        {
            // Given
            var fixture = new TuiFixture(new Size(2, 3));

            // When
            var result = fixture.Render(
                new BoxWidget().Title("X"));

            // Then
            result.ShouldBe(
                """
                ╭╮
                ││
                ╰╯
                """);
        }

        [Fact]
        public void Should_Preserve_Corners_When_Right_Title_Overflows()
        {
            // Given
            var fixture = new TuiFixture(new Size(8, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .Title("TooLongTitle", TitlePosition.Top, Justify.Right));

            // Then
            result.ShouldBe(
                """
                ╭TooLon╮
                │••••••│
                ╰──────╯
                """);
        }

        [Fact]
        public void Should_Let_Center_Win_On_Collision()
        {
            // Given
            var fixture = new TuiFixture(new Size(8, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .Title("LL", TitlePosition.Top, Justify.Left)
                    .Title("RR", TitlePosition.Top, Justify.Right)
                    .Title("CCCC", TitlePosition.Top, Justify.Center));

            // Then
            result.ShouldBe(
                """
                ╭LCCCCR╮
                │••••••│
                ╰──────╯
                """);
        }
    }

    public sealed class TheTitlePaddingProperty
    {
        [Fact]
        public void Should_Shift_Left_Title_Right_By_Padding()
        {
            // Given
            var fixture = new TuiFixture(new Size(12, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .TitlePadding(2)
                    .Title("Hi"));

            // Then
            result.ShouldBe(
                """
                ╭──Hi──────╮
                │••••••••••│
                ╰──────────╯
                """);
        }

        [Fact]
        public void Should_Shift_Right_Title_Left_By_Padding()
        {
            // Given
            var fixture = new TuiFixture(new Size(12, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .TitlePadding(2)
                    .Title("Hi", TitlePosition.Top, Justify.Right));

            // Then
            result.ShouldBe(
                """
                ╭──────Hi──╮
                │••••••••••│
                ╰──────────╯
                """);
        }

        [Fact]
        public void Should_Not_Shift_Center_Title()
        {
            // Given
            var fixture = new TuiFixture(new Size(12, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .TitlePadding(2)
                    .Title("Hi", TitlePosition.Top, Justify.Center));

            // Then
            result.ShouldBe(
                """
                ╭────Hi────╮
                │••••••••••│
                ╰──────────╯
                """);
        }

        [Fact]
        public void Should_Skip_Titles_When_Padding_Consumes_Border()
        {
            // Given
            var fixture = new TuiFixture(new Size(12, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .TitlePadding(99)
                    .Title("Hi"));

            // Then
            result.ShouldBe(
                """
                ╭──────────╮
                │••••••••••│
                ╰──────────╯
                """);
        }

        [Fact]
        public void Should_Treat_Negative_Padding_As_Zero()
        {
            // Given
            var fixture = new TuiFixture(new Size(12, 3));

            // When
            var result = fixture.Render(
                new BoxWidget()
                    .TitlePadding(-3)
                    .Title("Hi"));

            // Then
            result.ShouldBe(
                """
                ╭Hi────────╮
                │••••••••••│
                ╰──────────╯
                """);
        }
    }

    public sealed class TheStyleProperty
    {
        [Fact]
        public void Should_Apply_Title_Line_Style_To_Title_Text()
        {
            // Given
            var fixture = new TuiFixture(new AnsiTestTerminal(size: new Size(8, 3)));

            // When
            var result = fixture.Render(ctx =>
            {
                ctx.Render(
                    new BoxWidget()
                        .Title(TextLine.FromString("Hi", new Style(Color.Yellow))));
            });

            // Then
            result.ShouldContain("\e[38;5;11mHi");
        }

        [Fact]
        public void Should_Parse_Markup_From_MarkupTitle()
        {
            // Given
            var fixture = new TuiFixture(new AnsiTestTerminal(size: new Size(10, 3)));

            // When
            var result = fixture.Render(ctx =>
            {
                ctx.Render(
                    new BoxWidget()
                        .MarkupTitle("[yellow]Hi[/]"));
            });

            // Then
            result.ShouldContain("\e[38;5;11mHi");
        }

        [Fact]
        public void Should_Let_Title_Span_Style_Override_Box_Style()
        {
            // Given
            var fixture = new TuiFixture(new AnsiTestTerminal(size: new Size(10, 3)));

            // When
            var result = fixture.Render(ctx =>
            {
                ctx.Render(
                    new BoxWidget()
                        .Style(new Style(Color.Yellow))
                        .Title(TextLine.FromMarkup("[red]Hi[/]")));
            });

            // Then
            result.ShouldContain("\e[38;5;9mHi");
        }
    }
}
