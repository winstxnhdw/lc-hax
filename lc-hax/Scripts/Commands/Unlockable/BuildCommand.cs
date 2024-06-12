#region

using System;
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


        if (args[0].ToLower() == "suits")
            this.BuildSuits();
        else if (args[0].ToLower() == "all")
            this.BuildAllUnlockables(camera);
        else
            this.BuildSingleUnlockable(args[0], camera);
    }

    void BuildSingleUnlockable(string? unlockableName,  Camera camera) {
        if (unlockableName is null) {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        if (!unlockableName.FuzzyMatch(Helper.Unlockables.Keys, out string key)) {
            Chat.Print("Failed to find unlockable!");
            Chat.Print("Available unlockables: " + string.Join(", ", Helper.Unlockables.Keys.OrderBy(s => s)));
            Console.WriteLine("Available unlockables: " + string.Join(", ", Helper.Unlockables.Keys.OrderBy(s => s)));
            return;
        }

        var unlockable = Helper.Unlockables[key];
        this.BuildUnlockable(unlockable, camera);
    }

    void BuildAllUnlockables(Camera camera) {
        foreach (var item in Helper.Unlockables) {
            this.BuildUnlockable(item.Value, camera);
        }
    }

    void BuildSuits() {
        foreach (var suit in Helper.Suits) {
            Helper.BuyUnlockable(suit.Value);
        }
    }

    void BuildUnlockable(int UnlockableID, Camera camera) {

        var unlockable = Helper.GetUnlockableByID(UnlockableID);
        if (unlockable is null) {
            Chat.Print("Unlockable is not found!");
            return;
        }

        Helper.BuyUnlockable(UnlockableID);
        Helper.ReturnUnlockable(UnlockableID);

        Chat.Print($"Attempting to build a {unlockable.unlockableName}!");

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
