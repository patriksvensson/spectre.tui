namespace Spectre.Tui;

internal interface IKernelWorker
{
    bool IsRunning { get; }
    void Start();
    void Stop();
}