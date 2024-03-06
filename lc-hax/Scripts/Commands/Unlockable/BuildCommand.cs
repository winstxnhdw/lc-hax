using System.Linq;
using System.Collections.Generic;
using Hax;
using UnityEngine;

[Command("build")]
class BuildCommand : ICommand {
    static Dictionary<string, int>? Unlockables { get; set; }

    public void Execute(StringArray args) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.CurrentCamera is not Camera camera) return;
        if (args[0] is not string unlockableName) {
            Chat.Print("Usage: build <unlockable>");
            return;
        }

        BuildCommand.Unlockables ??=
            startOfRound.unlockablesList.unlockables.Select((unlockable, i) => (unlockable, i)).ToDictionary(
                pair => pair.unlockable.unlockableName.ToLower(),
                pair => pair.i
            );

        if (!unlockableName.FuzzyMatch(BuildCommand.Unlockables.Keys, out string key)) {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        Unlockable unlockable = (Unlockable)BuildCommand.Unlockables[key];
        Helper.BuyUnlockable(unlockable);
        Helper.ReturnUnlockable(unlockable);

        Chat.Print($"Attempting to build a {string.Join(' ', unlockable.ToString().Split('_')).ToTitleCase()}!");

        if (Helper.GetUnlockable(unlockable) is not PlaceableShipObject shipObject) {
            Chat.Print("Unlockable is not found or placeable!");
            return;
        }

        Vector3 newPosition = camera.transform.position + (camera.transform.forward * 3.0f);
        Vector3 newRotation = camera.transform.eulerAngles;
        newRotation.x = -90.0f;

        Helper.PlaceObjectAtPosition(
            shipObject,
            newPosition,
            newRotation
        );
    }
}
