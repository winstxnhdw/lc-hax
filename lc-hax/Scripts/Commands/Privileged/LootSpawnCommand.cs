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
            Chat.Print("Usage: give <item>");
            return;
        }
        itemName = itemName.Replace("_", " ");

        Item? scrap = Helper.GetItem(itemName);
        if (scrap is null) {
            Chat.Print($"{itemName} not found!");
            return;
        }

        Transform? target;
        if (Setting.EnablePhantom && Helper.CurrentCamera is Camera camera && camera.enabled) {
            target = camera.transform;
        }
        else {
            target = (PossessionMod.Instance != null && PossessionMod.Instance.IsPossessed)
                ? PossessionMod.Instance.PossessedEnemy?.transform
                : localPlayer.transform;
        }

        if(target is null) return;
        _ = Helper.SpawnItem(target.position, scrap);
    }
}


