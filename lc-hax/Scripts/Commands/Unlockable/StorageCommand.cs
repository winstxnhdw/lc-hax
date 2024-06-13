#region

using System;
using System.Collections.Generic;
using Hax;
using UnityEngine;

#endregion

[Command("storage")]
class StorageCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (Helper.CurrentCamera is not Camera camera) return;

        if (args.Length == 0) {
            Chat.Print("Usage: storage <unlockable|all>");
            string storableItems = this.StorableItems();
            if (storableItems.IsNullOrEmptyOrWhiteSpace()) {
                return;
            }

            Chat.Print("Items : " + storableItems);
            Console.WriteLine("Items : " + storableItems);
            return;
        }

        if (args[0].ToLower() == "all")
            this.StoreAllUnlockables();
        else
            this.StoreSingleUnlockable(args[0]);
    }

    string StorableItems() {
        PlaceableShipObject[] placed_items = Helper.FindObjects<PlaceableShipObject>();
        HashSet<string> unlockableNames = new();
        foreach (PlaceableShipObject item in placed_items) {
            UnlockableItem? unlockable = item.GetUnlockable();
            if (unlockable is not null) {
                if (!unlockable.CanBeReturnedToStorage()) continue;
                unlockableNames.Add(unlockable.unlockableName);
            }
        }

        return string.Join(", ", unlockableNames);
    }


    void StoreSingleUnlockable(string? unlockableName) {
        if (unlockableName is null) {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        if (!unlockableName.FuzzyMatch(Helper.Unlockables.Keys, out string key)) {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        this.StoreUnlockable(Helper.Unlockables[key]);
    }

    void StoreAllUnlockables() {
        foreach (KeyValuePair<string, int> item in Helper.Unlockables) this.StoreUnlockable(item.Value);
    }

    void StoreUnlockable(int UnlockableID) {
        UnlockableItem? unlockable = Helper.GetUnlockableByID(UnlockableID);
        if (unlockable is null) {
            Chat.Print("Failed to find unlockable!");
            return;
        }

        if (!unlockable.CanBeReturnedToStorage()) return;
        PlaceableShipObject? PlaceShipObject = Helper.GetPlaceableShipObject(unlockable);
        if (PlaceShipObject is null) {
            Chat.Print($"Failed to Return item : {unlockable.unlockableName}!");
            return;
        }

        Helper.ReturnUnlockableToStorage(PlaceShipObject);
    }
}
