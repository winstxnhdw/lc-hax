using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZLinq;

[Command("visit")]
sealed class VisitCommand : ICommand {
    static Dictionary<string, int>? Levels { get; set; }

    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.Terminal is not Terminal terminal)
            return Task.CompletedTask;
        if (Helper.StartOfRound is not StartOfRound startOfRound)
            return Task.CompletedTask;
        if (args[0] is not string moon) {
            Chat.Print("Usage: visit <moon>");
            return Task.CompletedTask;
        }

        if (!startOfRound.inShipPhase) {
            Chat.Print("You cannot use this command outside of the ship phase!");
            return Task.CompletedTask;
        }

        if (startOfRound.travellingToNewLevel) {
            Chat.Print("You cannot use this command while travelling to a new level!");
            return Task.CompletedTask;
        }

        VisitCommand.Levels ??= startOfRound.levels.AsValueEnumerable().ToDictionary(
            level => level.name[..(level.name.Length - "Level".Length)].ToLower(),
            level => level.levelID
        );

        if (!moon.FuzzyMatch(VisitCommand.Levels.Keys, out string key)) {
            Chat.Print("Failed to find moon!");
            return Task.CompletedTask;
        }

        startOfRound.ChangeLevelServerRpc(VisitCommand.Levels[key], terminal.groupCredits);
        Chat.Print($"Visiting {key.ToTitleCase()}!");
        return Task.CompletedTask;
    }
}
