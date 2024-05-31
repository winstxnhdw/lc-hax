using System.Collections.Generic;
using System.Linq;
using Hax;
using UnityEngine;

[Command("build")]
internal class BuildCommand : ICommand
{
    private static Dictionary<string, int>? Unlockables { get; set; }

    public void Execute(StringArray args)
    {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.CurrentCamera is not Camera camera) return;

        InitializeUnlockables(startOfRound);

        if (args.Length == 0)
        {
            Chat.Print("Usage: build <unlockable|all>");
            return;
        }

        if (args[0].ToLower() == "all")
        {
            BuildAllUnlockables(camera);
        }
        else
        {
            BuildSingleUnlockable(args[0], camera);
        }
    }

    private void InitializeUnlockables(StartOfRound startOfRound)
    {
        if (Unlockables != null) return;

        Unlockables = startOfRound.unlockablesList.unlockables
            .Select((unlockable, i) => (unlockable, i))
            .ToDictionary(
                pair => pair.unlockable.unlockableName.ToLower(),
                pair => pair.i
            );
    }

    private void BuildSingleUnlockable(string unlockableName, Camera camera)
    {
        if (!unlockableName.FuzzyMatch(Unlockables.Keys, out var key))
        {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        var unlockable = (Unlockable)Unlockables[key];
        BuildUnlockable(unlockable, camera);
    }

    private void BuildAllUnlockables(Camera camera)
    {
        foreach (var unlockable in Unlockables.Values.Select(index => (Unlockable)index))
        {
            BuildUnlockable(unlockable, camera);
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
