using System.Collections.Generic;
using System.Linq;
using Hax;
using UnityEngine;

[Command("build")]
internal class BuildCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.CurrentCamera is not Camera camera) return;

        if (args.Length == 0)
        {
            Chat.Print("Usage: build <unlockable|suits|all>");
            return;
        }

        var unlockables = InitializeUnlockables(startOfRound);

        if (args[0].ToLower() == "suits")
        {
            BuildSuits(unlockables);
        }
        else if (args[0].ToLower() == "all")
        {
            BuildAllUnlockables(unlockables, camera);
        }
        else
        {
            BuildSingleUnlockable(args[0], unlockables, camera);
        }
    }

    private Dictionary<string, Unlockable> InitializeUnlockables(StartOfRound startOfRound)
    {
        return startOfRound.unlockablesList.unlockables
            .Select((unlockable, i) => (unlockable, i))
            .ToDictionary(
                pair => pair.unlockable.unlockableName.ToLower(),
                pair => (Unlockable)pair.i
            );
    }

    private void BuildSingleUnlockable(string? unlockableName, Dictionary<string, Unlockable> unlockables, Camera camera)
    {
        if (unlockableName is null)
        {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        if (!unlockableName.FuzzyMatch(unlockables.Keys, out var key))
        {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        var unlockable = unlockables[key];
        BuildUnlockable(unlockable, camera);
    }

    private void BuildAllUnlockables(Dictionary<string, Unlockable> unlockables, Camera camera)
    {
        foreach (var unlockable in unlockables.Values)
        {
            BuildUnlockable(unlockable, camera);
        }
    }

    private void BuildSuits(Dictionary<string, Unlockable> unlockables)
    {
        var suitUnlockables = unlockables.Values.Where(u => u.ToString().EndsWith("_SUIT"));
        foreach (var suit in suitUnlockables)
        {
            Helper.BuyUnlockable(suit);
        }
    }

    private void BuildUnlockable(Unlockable unlockable, Camera camera)
    {
        Helper.BuyUnlockable(unlockable);
        Helper.ReturnUnlockable(unlockable);

        Chat.Print($"Attempting to build a {string.Join(' ', unlockable.ToString().Split('_')).ToTitleCase()}!");

        if (Helper.GetUnlockable(unlockable) is not PlaceableShipObject shipObject)
        {
            Chat.Print("Unlockable is not found or placeable!");
            return;
        }

        var newPosition = camera.transform.position + camera.transform.forward * 3.0f;
        var newRotation = camera.transform.eulerAngles;
        newRotation.x = -90.0f;

        Helper.PlaceObjectAtPosition(shipObject, newPosition, newRotation);
    }
}
