using System.Linq;
using GameNetcodeStuff;
using Hax;
using Unity.Netcode;
using UnityEngine;

[PrivilegedCommand("give")]
internal class LootSpawnCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        string? itemName = args[0];
        if (itemName is null) {
            Chat.Print("Usage: give <item> <amount>");
            return;
        }
        itemName = itemName.Replace("_", " ");

        Item? scrap = Helper.GetItem(itemName);
        if (scrap is null) {
            Chat.Print($"{itemName} not found!");
            return;
        }

        if (!args[1].TryParse(defaultValue: 1, result: out ulong amount)) {
            Chat.Print("Invalid amount!");
            return;
        }


        Transform? target = PossessionMod.Instance?.PossessedEnemy is not null
            ? PossessionMod.Instance.PossessedEnemy.transform
            : Setting.EnablePhantom ? Helper.CurrentCamera?.transform
                : localPlayer.transform;


        if (target is null) return;

        Vector3 spawnPosition = target.position + Vector3.up * 0.5f;

        if (amount != 1) {
            Helper.SpawnItems(spawnPosition, scrap, (int)amount);
        }
        else {
            _ = Helper.SpawnItem(spawnPosition, scrap);
        }
    }
}


