using System;

internal interface ICommand : IDisposable {
    void Execute(StringArray args);

    void IDisposable.Dispose() { }
}
