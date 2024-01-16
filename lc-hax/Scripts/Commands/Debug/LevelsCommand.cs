using System;
using Hax;

[DebugCommand("/levels")]
public class LevelsCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        Helper.StartOfRound?.levels.ForEach((i, level) =>
            Logger.Write($"{level.name} = {i}")
        );
    }
}
