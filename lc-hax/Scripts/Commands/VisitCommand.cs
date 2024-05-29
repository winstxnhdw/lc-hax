using System;
using System.Collections.Generic;
using System.Linq;
using Hax;

[Command("visit")]
internal class VisitCommand : ICommand
{
    private static Dictionary<string, int>? Levels { get; set; }

    public void Execute(StringArray args)
    {
        if (Helper.Terminal is not Terminal terminal) return;
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        Levels ??= startOfRound.levels.ToDictionary(
            level => level.name[..(level.name.Length - "Level".Length)].ToLower(),
            level => level.levelID
        );

        if (args[0] is not string moon)
        {
            Chat.Print("Usage: visit <moon>");
            Chat.Print("Moons: " + string.Join(", ", Levels.Keys));
            // dump to console as well
            Console.WriteLine("Moons: " + string.Join(", ", Levels.Keys));
            return;
        }

        if (!startOfRound.inShipPhase)
        {
            Chat.Print("You cannot use this command outside of the ship phase!");
            return;
        }

        if (startOfRound.travellingToNewLevel)
        {
            Chat.Print("You cannot use this command while travelling to a new level!");
            return;
        }


        if (!moon.FuzzyMatch(Levels.Keys, out var key))
        {
            Chat.Print("Failed to find moon!");
            return;
        }

        startOfRound.ChangeLevelServerRpc(Levels[key], terminal.groupCredits);
        Chat.Print($"Visiting {key.ToTitleCase()}!");
    }
}