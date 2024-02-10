using System.Linq;
using System.Collections.Generic;
using Hax;

[Command("visit")]
internal class VisitCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: visit <moon>");
            return;
        }

        if (Helper.Terminal is not Terminal terminal) {
            Chat.Print("Terminal not found!");
            return;
        }

        if (Helper.StartOfRound is not StartOfRound startOfRound) {
            Chat.Print("StartOfRound not found!");
            return;
        }

        if (!startOfRound.inShipPhase) {
            Chat.Print("You cannot use this command outside of the ship phase!");
            return;
        }

        if (startOfRound.travellingToNewLevel) {
            Chat.Print("You cannot use this command while travelling to a new level!");
            return;
        }

        Dictionary<string, int> levels = startOfRound.levels.ToDictionary(
            level => level.name[..(level.name.Length - "Level".Length)].ToLower(),
            level => level.levelID
        );

        string? key = Helper.FuzzyMatch(args[0]?.ToLower(), [.. levels.Keys]);

        if (string.IsNullOrWhiteSpace(key)) {
            Chat.Print("Failed to find moon!");
            return;
        }

        startOfRound.ChangeLevelServerRpc(levels[key], terminal.groupCredits);

        Chat.Print($"Visiting {key.ToTitleCase()}!");
    }
}
