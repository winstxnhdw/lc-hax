using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[Command("visit")]
class VisitCommand : ICommand {
    static Dictionary<string, int>? Levels { get; set; }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.Terminal is not Terminal terminal) return;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (args[0] is not string moon) {
            Chat.Print("Usage: visit <moon>");
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

        VisitCommand.Levels ??= startOfRound.levels.ToDictionary(
            level => level.name[..(level.name.Length - "Level".Length)].ToLower(),
            level => level.levelID
        );

        if (!moon.FuzzyMatch(VisitCommand.Levels.Keys, out string key)) {
            Chat.Print("Failed to find moon!");
            return;
        }

        startOfRound.ChangeLevelServerRpc(VisitCommand.Levels[key], terminal.groupCredits);
        Chat.Print($"Visiting {key.ToTitleCase()}!");
    }
}
