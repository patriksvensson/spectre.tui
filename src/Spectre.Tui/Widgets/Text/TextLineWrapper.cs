namespace Spectre.Tui;

internal static class TextLineWrapper
{
    public static IEnumerable<TextLine> Wrap(IEnumerable<TextLine> lines, int width)
    {
        if (width <= 0)
        {
            yield break;
        }

        foreach (var line in lines)
        {
            foreach (var wrapped in WrapLine(line, width))
            {
                yield return wrapped;
            }
        }
    }

    private static IEnumerable<TextLine> WrapLine(TextLine line, int width)
    {
        var current = new TextLine { Style = line.Style };
        var currentWidth = 0;
        var emitted = false;

        foreach (var span in line.Spans)
        {
            var spanWidth = span.GetWidth();

            if (currentWidth + spanWidth <= width)
            {
                current.Spans.Add(span);
                currentWidth += spanWidth;
                continue;
            }

            if (span.IsWhiteSpace)
            {
                if (TryFlush(ref current, line.Style, out var flushed))
                {
                    yield return flushed;
                    emitted = true;
                }

                currentWidth = 0;
                continue;
            }

            if (spanWidth <= width)
            {
                if (TryFlush(ref current, line.Style, out var flushed))
                {
                    yield return flushed;
                    emitted = true;
                }

                current.Spans.Add(span);
                currentWidth = spanWidth;
                continue;
            }

            if (TryFlush(ref current, line.Style, out var flushedHard))
            {
                yield return flushedHard;
                emitted = true;
            }

            currentWidth = 0;

            foreach (var (chunk, chunkWidth, isFinal) in HardBreak(span, width))
            {
                if (!isFinal)
                {
                    var chunkLine = new TextLine { Style = line.Style };
                    chunkLine.Spans.Add(chunk);
                    yield return chunkLine;
                    emitted = true;
                }
                else
                {
                    current.Spans.Add(chunk);
                    currentWidth = chunkWidth;
                }
            }
        }

        if (current.Spans.Count > 0 || !emitted)
        {
            yield return TrimTrailingWhitespace(current);
        }
    }

    public static TextLine TruncateWithEllipsis(TextLine line, int width)
    {
        if (width <= 0)
        {
            return new TextLine { Style = line.Style };
        }

        if (line.GetWidth() <= width)
        {
            return line;
        }

        var target = width - 1;
        var result = new TextLine { Style = line.Style };
        var currentWidth = 0;
        Style? ellipsisStyle = null;

        foreach (var span in line.Spans)
        {
            var spanWidth = span.GetWidth();
            ellipsisStyle = span.Style;

            if (currentWidth + spanWidth <= target)
            {
                result.Spans.Add(span);
                currentWidth += spanWidth;
                continue;
            }

            var buffer = new StringBuilder();
            var bufferWidth = 0;
            foreach (var grapheme in span.Text.Graphemes())
            {
                var graphemeWidth = grapheme.GetCellWidth();
                if (currentWidth + bufferWidth + graphemeWidth > target)
                {
                    break;
                }

                buffer.Append(grapheme);
                bufferWidth += graphemeWidth;
            }

            if (buffer.Length > 0)
            {
                result.Spans.Add(new TextSpan(buffer.ToString(), span.Style));
                currentWidth += bufferWidth;
            }

            break;
        }

        result.Spans.Add(new TextSpan("…", ellipsisStyle));
        return result;
    }

    private static IEnumerable<(TextSpan Span, int Width, bool IsFinal)> HardBreak(TextSpan span, int width)
    {
        var buffer = new StringBuilder();
        var bufferWidth = 0;

        foreach (var grapheme in span.Text.Graphemes())
        {
            var graphemeWidth = grapheme.GetCellWidth();

            if (graphemeWidth == 0)
            {
                buffer.Append(grapheme);
                continue;
            }

            if (bufferWidth + graphemeWidth > width && buffer.Length > 0)
            {
                yield return (new TextSpan(buffer.ToString(), span.Style), bufferWidth, IsFinal: false);
                buffer.Clear();
                bufferWidth = 0;
            }

            buffer.Append(grapheme);
            bufferWidth += graphemeWidth;
        }

        yield return (new TextSpan(buffer.ToString(), span.Style), bufferWidth, IsFinal: true);
    }

    private static TextLine TrimTrailingWhitespace(TextLine line)
    {
        while (line.Spans.Count > 0 && line.Spans[^1].IsWhiteSpace)
        {
            line.Spans.RemoveAt(line.Spans.Count - 1);
        }

        return line;
    }

    private static bool TryFlush(ref TextLine current, Style? style, out TextLine flushed)
    {
        var trimmed = TrimTrailingWhitespace(current);
        current = new TextLine { Style = style };

        if (trimmed.Spans.Count == 0)
        {
            flushed = default!;
            return false;
        }

        flushed = trimmed;
        return true;
    }
}
