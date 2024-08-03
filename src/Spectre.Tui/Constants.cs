namespace Spectre.Tui;

internal static class Constants
{
#if DEBUG
    public const bool IsDebug = true;
#else
    public const bool IsDebug = false;
#endif
}