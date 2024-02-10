using System.Collections;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("/bombard")]
internal class BombardCommand : ICommand {
    JetpackItem[] GetAvailableJetpacks() =>
        Helper.FindObjects<JetpackItem>()
              .Where(jetpack => !jetpack.Reflect().GetInternalField<bool>("jetpackBroken"))
              .ToArray();

    IEnumerator BombardAsync(PlayerControllerB player, Transform targetTransform, JetpackItem[] jetpacks) {
        float currentWeight = player.carryWeight;

        foreach (JetpackItem jetpack in jetpacks) {
            player.GrabObject(jetpack);
            yield return new WaitUntil(() => player.ItemSlots[player.currentItemSlot] == jetpack);

            const float bombardRadius = 10.0f;
            Vector3 randomDirection = Random.insideUnitSphere * bombardRadius;
            player.DiscardHeldObject(placeObject: true, placePosition: targetTransform.position + randomDirection);

            Helper.CreateComponent<WaitForBehaviour>()
                  .SetPredicate(time => time > 1.5f)
                  .Init(() => Helper.ShortDelay(jetpack.ExplodeJetpackServerRpc));
        }

        player.carryWeight = currentWeight;
    }

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0) {
            Chat.Print("Usage: /bombard <player>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        JetpackItem[] jetpacks = this.GetAvailableJetpacks();

        if (jetpacks.Length is 0) {
            Chat.Print("A usable jetpack is required to use this command!");
            return;
        }

        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => this.BombardAsync(localPlayer, targetPlayer.transform, jetpacks));
    }
}
