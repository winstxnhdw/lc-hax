using System;

public interface ICommand : IDisposable {
    void Execute(StringArray args);

    void IDisposable.Dispose() { }
}
