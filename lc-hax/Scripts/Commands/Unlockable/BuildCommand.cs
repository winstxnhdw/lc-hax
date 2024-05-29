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
        if (args[0] is not string unlockableName)
        {
            Chat.Print("Usage: build <unlockable>");
            return;
        }

        Unlockables ??=
            startOfRound.unlockablesList.unlockables.Select((unlockable, i) => (unlockable, i)).ToDictionary(
                pair => pair.unlockable.unlockableName.ToLower(),
                pair => pair.i
            );

        if (!unlockableName.FuzzyMatch(Unlockables.Keys, out var key))
        {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        var unlockable = (Unlockable)Unlockables[key];
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

        Helper.PlaceObjectAtPosition(
            shipObject,
            newPosition,
            newRotation
        );
    }
}