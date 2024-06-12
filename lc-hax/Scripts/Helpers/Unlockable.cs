#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Hax;

static partial class Helper {
    static Dictionary<int, UnlockableItem> _UnlockableItems;
    static Dictionary<string, int> _Unlockables;
    static Dictionary<string, int> _Suits;

    /// <summary>
    ///     Gets the dictionary of unlockable items (ID, UnlockableItem).
    /// </summary>
    internal static Dictionary<int, UnlockableItem> UnlockableItems {
        get {
            if (_UnlockableItems == null) {
                if (StartOfRound is not StartOfRound startOfRound) return null;
                for (int i = 0; i < startOfRound.unlockablesList.unlockables.Count; i++) {
                    int index = i;
                    UnlockableItem? unlockable = startOfRound.unlockablesList.unlockables[index];
                    if (_UnlockableItems == null) _UnlockableItems = new Dictionary<int, UnlockableItem>();
                    Console.WriteLine($"Unlockable ID: {index} - Unlockable Name: {unlockable.unlockableName}");
                    _ = _UnlockableItems.TryAdd(index, unlockable);
                }
            }

            return _UnlockableItems;
        }
    }

    /// <summary>
    ///     Gets the available unlockables (Name, ID)
    /// </summary>
    internal static Dictionary<string, int> Unlockables {
        get {
            if (_Unlockables == null) {
                if (UnlockableItems == null || !UnlockableItems.Any()) return null;
                foreach (KeyValuePair<int, UnlockableItem> unlockable in UnlockableItems) {
                    _Unlockables ??= new Dictionary<string, int>();
                    _ = _Unlockables.TryAdd(unlockable.Value.unlockableName, unlockable.Key);
                }
            }

            return _Unlockables;
        }
    }

    /// <summary>
    /// Gets all the available suits (Name, ID)
    /// </summary>
    internal static Dictionary<string, int> Suits {
        get {
            if (_Suits == null) {
                if (UnlockableItems == null || !UnlockableItems.Any()) return null;
                foreach (KeyValuePair<int, UnlockableItem> unlockable in UnlockableItems) {
                    if (!unlockable.Value.unlockableName.ToLower().EndsWith(" suit")) continue;
                    _Suits ??= new Dictionary<string, int>();
                    _ = _Suits.TryAdd(unlockable.Value.unlockableName, unlockable.Key);
                }
            }

            return _Suits;
        }
    }



    /// <summary>
    ///     Gets the unlockable ID for a given unlockable item.
    /// </summary>
    /// <param name="unlockable">The unlockable item.</param>
    /// <returns>The unlockable ID or null if not found.</returns>
    internal static int? GetUnlockableID(this UnlockableItem unlockable) {
        KeyValuePair<int, UnlockableItem>? unlockableEntry =
            UnlockableItems.FirstOrDefaultNull(x => x.Value == unlockable);
        return unlockableEntry?.Key ?? null;
    }

    /// <summary>
    ///     Looks up a unlockable item by its ID.
    /// </summary>
    /// <param name="Id">The unlockable ID.</param>
    /// <returns>The unlockable item or null if not found.</returns>
    internal static UnlockableItem GetUnlockableByID(int Id) =>
        UnlockableItems.TryGetValue(Id, out UnlockableItem? item) ? item : null;

    /// <summary>
    ///     Looks up a unlockable item by its name.
    /// </summary>
    /// <param name="Name">The unlockable name.</param>
    /// <returns>The unlockable item or null if not found.</returns>
    internal static UnlockableItem GetUnlockableByName(string Name) {
        KeyValuePair<int, UnlockableItem>? unlockableEntry =
            UnlockableItems.FirstOrDefaultNull(x => x.Value.unlockableName.ToLower() == Name.ToLower());
        return unlockableEntry?.Value;
    }

    /// <summary>
    ///     Checks if a given unlockable item matches the specified unlockable ID.
    /// </summary>
    /// <param name="unlockable">The unlockable item.</param>
    /// <param name="unlockableId">The unlockable ID to compare.</param>
    /// <returns>True if the unlockable matches the ID, false otherwise.</returns>
    internal static bool IsUnlockable(this UnlockableItem unlockable, int unlockableId) {
        // Look it in the dictionary based off the index
        UnlockableItem? unlockableItem = GetUnlockableByID(unlockableId);
        return unlockableItem == unlockable;
    }

    /// <summary>
    ///     Buys a unlockable item by its ID.
    /// </summary>
    /// <param name="unlockableId">The unlockable ID to buy.</param>
    internal static void BuyUnlockable(int unlockableId) {
        if (Terminal is not Terminal terminal) return;

        UnlockableItem? unlockable = GetUnlockableByID(unlockableId);
        if (unlockable == null) return;
        StartOfRound?.BuyShipUnlockableServerRpc(
            unlockableId,
            terminal.groupCredits
        );
    }

    /// <summary>
    ///     Buys a unlockable item by its name.
    /// </summary>
    /// <param name="unlockableName">The unlockable name to buy.</param>
    internal static void BuyUnlockable(string unlockableName) {
        if (Terminal is not Terminal terminal) return;
        UnlockableItem? item = GetUnlockableByName(unlockableName);
        if (item == null) return;
        int? id = item.GetUnlockableID();
        if (id == null) return;
        StartOfRound?.BuyShipUnlockableServerRpc(
            id.Value,
            terminal.groupCredits
        );
    }

    /// <summary>
    ///     Returns a previously bought unlockable item to storage.
    /// </summary>
    /// <param name="unlockable">The unlockable item to return.</param>
    internal static void ReturnUnlockable(UnlockableItem unlockable) {
        if (StartOfRound is not StartOfRound startOfRound) return;
        int? UnlockableID = unlockable.GetUnlockableID();
        if (UnlockableID == null) return;
        startOfRound.ReturnUnlockableFromStorageServerRpc(UnlockableID.Value);
    }

    // make variants using name and id
    internal static void ReturnUnlockable(string unlockableName) {
        if (Terminal is not Terminal terminal) return;
        UnlockableItem? item = GetUnlockableByName(unlockableName);
        if (item == null) return;
        int? id = item.GetUnlockableID();
        if (id == null) return;
        StartOfRound?.ReturnUnlockableFromStorageServerRpc(id.Value);
    }

    internal static void ReturnUnlockable(int unlockableId) {
        if (Terminal is not Terminal terminal) return;

        UnlockableItem? unlockable = GetUnlockableByID(unlockableId);
        if (unlockable == null) return;
        StartOfRound?.ReturnUnlockableFromStorageServerRpc(unlockableId);
    }


    /// <summary>
    ///     Gets the unlockable item associated with a specific unlockable ID.
    /// </summary>
    /// <param name="unlockable">The unlockable item.</param>
    /// <returns>The placeable ship object associated with the unlockable.</returns>
    internal static PlaceableShipObject? GetUnlockable(UnlockableItem unlockable) =>
        FindObjects<PlaceableShipObject>()
            .FirstOrDefault(placeableObject => unlockable.IsUnlockable(placeableObject.unlockableID));

    /// <summary>
    ///     Gets the unlockable item associated with a specific unlockable ID.
    /// </summary>
    /// <param name="unlockableId">The unlockable item ID.</param>
    /// <returns>The placeable ship object associated with the unlockable.</returns
    internal static PlaceableShipObject? GetUnlockable(int unlockableId) {
        UnlockableItem? unlockable = GetUnlockableByID(unlockableId);
        if (unlockable == null) return null;
        return GetUnlockable(unlockable);
    }

    /// <summary>
    ///     Gets the unlockable item associated with a specific unlockable ID.
    /// </summary>
    /// <param name="unlockableId">The unlockable item Name.</param>
    /// <returns>The placeable ship object associated with the unlockable.</returns
    internal static PlaceableShipObject? GetUnlockable(string unlockableName) {
        UnlockableItem? unlockable = GetUnlockableByName(unlockableName);
        if (unlockable == null) return null;
        return GetUnlockable(unlockable);
    }

    /// <summary>
    /// Verifies if a given unlockable item is a Unlockable Item.
    /// </summary>
    /// <param name="unlockable"></param>
    /// <param name="unlockableId"></param>
    /// <returns></returns>
    internal static bool IsUnlockable(this int unlockable, UnlockableItem unlockableId) {

        UnlockableItem? unlockableItem = GetUnlockableByID(unlockable);
        return unlockableItem != null;
    }

}
