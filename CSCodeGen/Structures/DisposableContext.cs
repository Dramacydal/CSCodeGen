namespace CSCodeGen.Structures;

public class DisposableContext : IDisposable
{
    private readonly Action _dispose;

    public DisposableContext(Action init, Action dispose)
    {
        init?.Invoke();
        _dispose = dispose;
    }

    public void Dispose()
    {
        _dispose?.Invoke();
    }
}