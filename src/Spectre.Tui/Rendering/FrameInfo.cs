namespace Spectre.Tui;

/// <summary>
/// Contains information about the current frame.
/// </summary>
public readonly record struct FrameInfo
{
    /// <summary>
    /// Gets the elapsed time since last frame.
    /// </summary>
    public TimeSpan FrameTime { get; }

    /// <summary>
    /// Gets the elapsed time since start.
    /// </summary>
    public TimeSpan Elapsed { get; }

    /// <summary>
    /// Gets the number of frames per second.
    /// </summary>
    public double Fps { get; }

    /// <summary>
    /// Gets the number of cells that changed since the previous frame.
    /// </summary>
    public int DiffCount { get; }

    public FrameInfo(TimeSpan frameTime, TimeSpan elapsed, int diffCount)
    {
        FrameTime = frameTime;
        Elapsed = elapsed;
        DiffCount = diffCount;
        Fps = TimeSpan.FromSeconds(1) / FrameTime;
    }
}