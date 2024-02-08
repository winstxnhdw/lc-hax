using System;
using UnityEngine;
using GameNetcodeStuff;
using Hax;

[Command("/bomb")]
internal class BombCommand : ICommand {
    JetpackItem? GetAvailableJetpack(PlayerControllerB localPlayer) =>
        Helper.FindObjects<JetpackItem>()
              .First(jetpack => !jetpack.isHeld || jetpack.playerHeldBy == localPlayer);

    Action BlowUpLocation(PlayerControllerB localPlayer, Vector3 targetPosition, JetpackItem jetpack) => () => {
        localPlayer.DiscardHeldObject(placePosition: targetPosition);
        jetpack.ExplodeJetpackServerRpc();
    };

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0) {
            Chat.Print("Usage: /bomb <player>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (this.GetAvailableJetpack(localPlayer) is not JetpackItem jetpack) {
            Chat.Print("A free jetpack is required to use this command!");
            return;
        }

        Helper.CreateComponent<WaitForBehaviour>()
              .SetPredicate(() => localPlayer.ItemSlots[localPlayer.currentItemSlot] == jetpack)
              .Init(this.BlowUpLocation(localPlayer, targetPlayer.transform.position, jetpack));
    }
}
