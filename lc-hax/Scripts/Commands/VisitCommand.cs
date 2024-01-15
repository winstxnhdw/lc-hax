using System;
using System.Linq;
using System.Collections.Generic;
using Hax;

[Command("/visit")]
public class VisitCommand : ICommand {
    public void Execute(ReadOnlySpan<string> args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /visit <moon>");
            return;
        }

        if (!Helper.Terminal.IsNotNull(out Terminal terminal)) {
            Chat.Print("Terminal not found!");
            return;
        }

        if (Helper.StartOfRound is not StartOfRound startOfRound) {
            Chat.Print("StartOfRound not found!");
            return;
        }

        Dictionary<string, int> levels = startOfRound.levels.ToDictionary(
            level => level.name[..(level.name.Length - "Level".Length)].ToLower(),
            level => level.levelID
        );

        string key = Helper.FuzzyMatch(args[0].ToLower(), [.. levels.Keys]);
        Helper.StartOfRound?.ChangeLevelServerRpc(levels[key], terminal.groupCredits);

        Chat.Print($"Visiting {key.ToTitleCase()}!");
    }
}
