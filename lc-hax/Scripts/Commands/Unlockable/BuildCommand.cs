#region

using System.Collections.Generic;
using System.Linq;
using Hax;
using UnityEngine;

#endregion

[Command("build")]
class BuildCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.CurrentCamera is not Camera camera) return;

        if (args.Length == 0) {
            Chat.Print("Usage: build <unlockable|suits|all>");
            return;
        }

        Dictionary<string, Unlockable> unlockables = this.InitializeUnlockables(startOfRound);

        if (args[0].ToLower() == "suits")
            this.BuildSuits(unlockables);
        else if (args[0].ToLower() == "all")
            this.BuildAllUnlockables(unlockables, camera);
        else
            this.BuildSingleUnlockable(args[0], unlockables, camera);
    }

    Dictionary<string, Unlockable> InitializeUnlockables(StartOfRound startOfRound) =>
        startOfRound.unlockablesList.unlockables
            .Select((unlockable, i) => (unlockable, i))
            .ToDictionary(
                pair => pair.unlockable.unlockableName.ToLower(),
                pair => (Unlockable)pair.i
            );

    void BuildSingleUnlockable(string? unlockableName, Dictionary<string, Unlockable> unlockables, Camera camera) {
        if (unlockableName is null) {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        if (!unlockableName.FuzzyMatch(unlockables.Keys, out string key)) {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        Unlockable unlockable = unlockables[key];
        this.BuildUnlockable(unlockable, camera);
    }

    void BuildAllUnlockables(Dictionary<string, Unlockable> unlockables, Camera camera) {
        foreach (Unlockable unlockable in unlockables.Values) this.BuildUnlockable(unlockable, camera);
    }

    void BuildSuits(Dictionary<string, Unlockable> unlockables) {
        IEnumerable<Unlockable> suitUnlockables = unlockables.Values.Where(u => u.ToString().EndsWith("_SUIT"));
        foreach (Unlockable suit in suitUnlockables) Helper.BuyUnlockable(suit);
    }

    void BuildUnlockable(Unlockable unlockable, Camera camera) {
        Helper.BuyUnlockable(unlockable);
        Helper.ReturnUnlockable(unlockable);

        Chat.Print($"Attempting to build a {string.Join(' ', unlockable.ToString().Split('_')).ToTitleCase()}!");

        if (Helper.GetUnlockable(unlockable) is not PlaceableShipObject shipObject) {
            Chat.Print("Unlockable is not found or placeable!");
            return;
        }

        Vector3 newPosition = camera.transform.position + camera.transform.forward * 3.0f;
        Vector3 newRotation = camera.transform.eulerAngles;
        newRotation.x = -90.0f;

        Helper.PlaceObjectAtPosition(shipObject, newPosition, newRotation);
    }
}
