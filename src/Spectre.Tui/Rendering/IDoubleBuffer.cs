namespace Spectre.Tui;

internal interface IDoubleBuffer
{
    Buffer Front { get; }
    Buffer Back { get; }
}