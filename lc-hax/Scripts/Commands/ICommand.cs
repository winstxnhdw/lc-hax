using System;

public interface ICommand {
    void Execute(ReadOnlySpan<string> args);
}
