using Spectre.Console;

namespace Spectre.Tui.Tests;

public sealed class ParagraphTests
{
    public sealed class TheConstructor
    {
        [Fact]
        public void Should_Set_Text()
        {
            // Given, When
            var paragraph = Paragraph.FromString("Hello World", Color.Red);

            // Then
            paragraph.Lines.Count.ShouldBe(1);
        }

        [Fact]
        public void Should_Set_Style()
        {
            // Given, When
            var paragraph = Paragraph.FromString("Hello World", Color.Red);

            // Then
            paragraph.Style.ShouldBe(new Style
            {
                Foreground = Color.Red,
            });
        }

        [Fact]
        public void Should_Split_Lines()
        {
            // Given, When
            var paragraph = Paragraph.FromString("Hello\nWorld", Color.Red);

            // Then
            paragraph.Lines.Count.ShouldBe(2);
            paragraph.Lines[0].Spans[0].Text.ShouldBe("Hello");
            paragraph.Lines[1].Spans[0].Text.ShouldBe("World");
        }

        [Fact]
        public void Should_Default_To_Left_Alignment()
        {
            // Given, When
            var paragraph = new Paragraph();

            // Then
            paragraph.Alignment.ShouldBe(Justify.Left);
        }

        [Fact]
        public void Should_Default_To_Top_Vertical_Alignment()
        {
            // Given, When
            var paragraph = new Paragraph();

            // Then
            paragraph.VerticalAlignment.ShouldBe(VerticalAlignment.Top);
        }
    }

    public sealed class TheGetWidthMethod
    {
        [Fact]
        public void Should_Return_Maximum_Line_Width()
        {
            // Given
            var paragraph = Paragraph.FromString("Hello\nWorld!");

            // When
            var result = paragraph.GetWidth();

            // Then
            result.ShouldBe(6);
        }
    }

    public sealed class TheGetWrappedHeightMethod
    {
        [Fact]
        public void Should_Return_Wrapped_Line_Count()
        {
            // Given
            var paragraph = Paragraph.FromString("Hello wonderful world");

            // When
            var result = paragraph.GetWrappedHeight(10);

            // Then
            result.ShouldBe(3);
        }
    }

    public sealed class TheRenderMethod
    {
        [Fact]
        public void Should_Wrap_At_Whitespace()
        {
            // Given
            var fixture = new TuiFixture(new Size(10, 3));

            // When
            var result = fixture.Render(
                Paragraph.FromString("Hello wonderful world"));

            // Then
            result.ShouldBe(
                """
                Hello•••••
                wonderful•
                world•••••
                """);
        }

        [Fact]
        public void Should_Drop_Whitespace_At_Wrap_Boundary()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 2));

            // When
            var result = fixture.Render(
                Paragraph.FromString("foo bar"));

            // Then
            result.ShouldBe(
                """
                foo••
                bar••
                """);
        }

        [Fact]
        public void Should_Hard_Break_Long_Words()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 4));

            // When
            var result = fixture.Render(
                Paragraph.FromString("abcdefghijklmnop"));

            // Then
            result.ShouldBe(
                """
                abcde
                fghij
                klmno
                p••••
                """);
        }

        [Fact]
        public void Should_Truncate_Past_Viewport_Height()
        {
            // Given
            var fixture = new TuiFixture(new Size(3, 2));

            // When
            var result = fixture.Render(
                Paragraph.FromString("aa bb cc dd"));

            // Then
            result.ShouldBe(
                """
                aa•
                bb•
                """);
        }

        [Fact]
        public void Should_Honor_Embedded_Line_Breaks()
        {
            // Given
            var fixture = new TuiFixture(new Size(10, 2));

            // When
            var result = fixture.Render(
                Paragraph.FromString("a b\nc d"));

            // Then
            result.ShouldBe(
                """
                a•b•••••••
                c•d•••••••
                """);
        }

        [Fact]
        public void Should_Preserve_Empty_Lines_Between_Content()
        {
            // Given
            var fixture = new TuiFixture(new Size(3, 3));

            // When
            var result = fixture.Render(
                Paragraph.FromString("a\n\nb"));

            // Then
            result.ShouldBe(
                """
                a••
                •••
                b••
                """);
        }

        [Fact]
        public void Should_Center_Align()
        {
            // Given
            var fixture = new TuiFixture(new Size(10, 1));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi")
                    .Alignment(Justify.Center));

            // Then
            result.ShouldBe("••••hi••••");
        }

        [Fact]
        public void Should_Right_Align()
        {
            // Given
            var fixture = new TuiFixture(new Size(10, 1));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi")
                    .Alignment(Justify.Right));

            // Then
            result.ShouldBe("••••••••hi");
        }

        [Fact]
        public void Should_Left_Align_By_Default()
        {
            // Given
            var fixture = new TuiFixture(new Size(10, 1));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi"));

            // Then
            result.ShouldBe("hi••••••••");
        }
    }

    public sealed class TheVerticalAlignmentProperty
    {
        [Fact]
        public void Should_Top_Align_By_Default()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 3));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi"));

            // Then
            result.ShouldBe(
                """
                hi•••
                •••••
                •••••
                """);
        }

        [Fact]
        public void Should_Middle_Align_When_Vertical_Alignment_Is_Middle()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 3));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi")
                    .AlignedMiddle());

            // Then
            result.ShouldBe(
                """
                •••••
                hi•••
                •••••
                """);
        }

        [Fact]
        public void Should_Bottom_Align_When_Vertical_Alignment_Is_Bottom()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 3));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi")
                    .AlignedBottom());

            // Then
            result.ShouldBe(
                """
                •••••
                •••••
                hi•••
                """);
        }

        [Fact]
        public void Should_Floor_Middle_Offset_With_Even_Slack()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 4));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi")
                    .AlignedMiddle());

            // Then
            result.ShouldBe(
                """
                •••••
                hi•••
                •••••
                •••••
                """);
        }

        [Fact]
        public void Should_Middle_Align_Multiple_Lines()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 5));

            // When
            var result = fixture.Render(
                Paragraph.FromString("a\nb\nc")
                    .AlignedMiddle());

            // Then
            result.ShouldBe(
                """
                •••••
                a••••
                b••••
                c••••
                •••••
                """);
        }

        [Fact]
        public void Should_Bottom_Align_Multiple_Lines()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 5));

            // When
            var result = fixture.Render(
                Paragraph.FromString("a\nb\nc")
                    .AlignedBottom());

            // Then
            result.ShouldBe(
                """
                •••••
                •••••
                a••••
                b••••
                c••••
                """);
        }

        [Fact]
        public void Should_Drop_Top_Lines_When_Bottom_Aligned_Overflows()
        {
            // Given
            var fixture = new TuiFixture(new Size(3, 2));

            // When
            var result = fixture.Render(
                Paragraph.FromString("aa bb cc dd")
                    .AlignedBottom());

            // Then
            result.ShouldBe(
                """
                cc•
                dd•
                """);
        }

        [Fact]
        public void Should_Not_Offset_When_Content_Equals_Viewport_Height()
        {
            // Given
            var fixture = new TuiFixture(new Size(3, 2));

            // When
            var result = fixture.Render(
                Paragraph.FromString("aa\nbb")
                    .AlignedBottom());

            // Then
            result.ShouldBe(
                """
                aa•
                bb•
                """);
        }

        [Fact]
        public void Should_Combine_Horizontal_And_Vertical_Alignment()
        {
            // Given
            var fixture = new TuiFixture(new Size(6, 3));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi")
                    .Centered()
                    .AlignedMiddle());

            // Then
            result.ShouldBe(
                """
                ••••••
                ••hi••
                ••••••
                """);
        }

        [Fact]
        public void Should_Bottom_Align_With_Crop_Overflow()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 3));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi")
                    .Cropped()
                    .AlignedBottom());

            // Then
            result.ShouldBe(
                """
                •••••
                •••••
                hi•••
                """);
        }

        [Fact]
        public void Should_Bottom_Align_With_Ellipsis_Overflow()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 3));

            // When
            var result = fixture.Render(
                Paragraph.FromString("Hello wonderful world")
                    .Ellipsis()
                    .AlignedBottom());

            // Then
            result.ShouldBe(
                """
                •••••
                •••••
                Hell…
                """);
        }

        [Fact]
        public void Should_Render_Nothing_When_Lines_Empty()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 3));

            // When
            var result = fixture.Render(
                new Paragraph().AlignedBottom());

            // Then
            result.ShouldBe(
                """
                •••••
                •••••
                •••••
                """);
        }
    }

    public sealed class TheOverflowProperty
    {
        [Fact]
        public void Should_Default_To_Fold()
        {
            // Given, When
            var paragraph = new Paragraph();

            // Then
            paragraph.Overflow.ShouldBe(Overflow.Fold);
        }

        [Fact]
        public void Should_Crop_Long_Line_When_Crop()
        {
            // Given
            var fixture = new TuiFixture(new Size(5, 1));

            // When
            var result = fixture.Render(
                Paragraph.FromString("Hello wonderful world")
                    .Overflow(Overflow.Crop));

            // Then
            result.ShouldBe("Hello");
        }

        [Fact]
        public void Should_Not_Wrap_When_Crop()
        {
            // Given
            var fixture = new TuiFixture(new Size(10, 3));

            // When
            var result = fixture.Render(
                Paragraph.FromString("aa bb\ncc dd ee\nff")
                    .Overflow(Overflow.Crop));

            // Then
            result.ShouldBe(
                """
                aa•bb•••••
                cc•dd•ee••
                ff••••••••
                """);
        }

        [Fact]
        public void Should_Append_Ellipsis_When_Truncating()
        {
            // Given
            var fixture = new TuiFixture(new Size(8, 1));

            // When
            var result = fixture.Render(
                Paragraph.FromString("Hello wonderful world")
                    .Overflow(Overflow.Ellipsis));

            // Then
            result.ShouldBe("Hello•w…");
        }

        [Fact]
        public void Should_Not_Append_Ellipsis_When_Line_Fits()
        {
            // Given
            var fixture = new TuiFixture(new Size(10, 1));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi")
                    .Overflow(Overflow.Ellipsis));

            // Then
            result.ShouldBe("hi••••••••");
        }

        [Fact]
        public void Should_Render_Only_Ellipsis_When_Width_Is_One()
        {
            // Given
            var fixture = new TuiFixture(new Size(1, 1));

            // When
            var result = fixture.Render(
                Paragraph.FromString("Hello")
                    .Overflow(Overflow.Ellipsis));

            // Then
            result.ShouldBe("…");
        }

        [Fact]
        public void Should_Return_Line_Count_For_Crop_From_GetWrappedHeight()
        {
            // Given
            var paragraph = Paragraph.FromString("Hello wonderful world")
                .Overflow(Overflow.Crop);

            // When
            var result = paragraph.GetWrappedHeight(5);

            // Then
            result.ShouldBe(1);
        }

        [Fact]
        public void Should_Return_Line_Count_For_Ellipsis_From_GetWrappedHeight()
        {
            // Given
            var paragraph = Paragraph.FromString("a\nb\nc")
                .Overflow(Overflow.Ellipsis);

            // When
            var result = paragraph.GetWrappedHeight(1);

            // Then
            result.ShouldBe(3);
        }

        [Fact]
        public void Should_Right_Align_Cropped_Short_Line()
        {
            // Given
            var fixture = new TuiFixture(new Size(10, 1));

            // When
            var result = fixture.Render(
                Paragraph.FromString("hi")
                    .Overflow(Overflow.Crop)
                    .Alignment(Justify.Right));

            // Then
            result.ShouldBe("••••••••hi");
        }
    }

    public sealed class TheFromMarkupMethod
    {
        [Fact]
        public void Should_Parse_Markup()
        {
            // Given
            var fixture = new TuiFixture(new AnsiTestTerminal());

            // When
            var result = fixture.Render(ctx =>
            {
                ctx.Render(
                    Paragraph.FromMarkup(
                        "[yellow]Hello[/] World"));
            });

            // Then
            result.ShouldBe("\e[1;1H\e[0m\e[38;5;11mHello\e[1;7H\e[0mWorld");
        }

        [Fact]
        public void Should_Preserve_Style_After_Wrapping()
        {
            // Given
            var fixture = new TuiFixture(
                new AnsiTestTerminal(size: new Size(5, 2)));

            // When
            var result = fixture.Render(ctx =>
            {
                ctx.Render(
                    Paragraph.FromMarkup(
                        "[yellow]aaa[/] [yellow]bbb[/]"));
            });

            // Then
            result.ShouldContain("\e[38;5;11maaa");
            result.ShouldContain("\e[38;5;11mbbb");
        }
    }

    public sealed class TheStyleProperty
    {
        [Fact]
        public void Should_Apply_Paragraph_Style_To_Unstyled_Text()
        {
            // Given
            var fixture = new TuiFixture(new AnsiTestTerminal());

            // When
            var result = fixture.Render(ctx =>
            {
                ctx.Render(
                    Paragraph.FromMarkup("Hello")
                        .Style(new Style(Color.Yellow)));
            });

            // Then
            result.ShouldContain("\e[38;5;11mHello");
        }

        [Fact]
        public void Should_Let_Span_Style_Override_Paragraph_Style()
        {
            // Given
            var fixture = new TuiFixture(new AnsiTestTerminal());

            // When
            var result = fixture.Render(ctx =>
            {
                ctx.Render(
                    Paragraph.FromMarkup("[red]Hello[/]")
                        .Style(new Style(Color.Yellow)));
            });

            // Then
            result.ShouldContain("\e[38;5;9mHello");
        }

        [Fact]
        public void Should_Combine_Paragraph_Style_Decoration_With_Span_Style()
        {
            // Given
            var fixture = new TuiFixture(new AnsiTestTerminal());

            // When
            var result = fixture.Render(ctx =>
            {
                ctx.Render(
                    Paragraph.FromMarkup("[red]Hello[/]")
                        .Style(new Style(decoration: Decoration.Bold)));
            });

            // Then
            result.ShouldContain("\e[1m");
            result.ShouldContain("\e[38;5;9mHello");
        }
    }
}
