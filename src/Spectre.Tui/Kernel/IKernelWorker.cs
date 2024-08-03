namespace Spectre.Tui;

internal interface IKernelWorker
{
    string Name { get; }
    bool IsRunning { get; }

    void Start();
    void Stop();
}