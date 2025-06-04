using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("bombard")]
class BombardCommand : ICommand, IJetpack {
    /// <summary>
    /// Grab and discard jetpacks to a random location of the same elevation near the target player.
    /// If the target player is far away, it may take a while for the jetpacks to reach the player.
    /// The jetpacks will only explode if they within 5 units of the target player.
    /// </summary>
    static IEnumerator BombardAsync(PlayerControllerB player, Transform targetTransform, JetpackItem[] jetpacks) {
        float currentWeight = player.carryWeight;

        foreach (JetpackItem jetpack in jetpacks) {
            if (!player.GrabObject(jetpack)) continue;
            yield return new WaitUntil(() => player.ItemSlots[player.currentItemSlot] == jetpack);

            const float bombardRadius = 10.0f;
            Vector2 randomDirection = Random.insideUnitCircle * bombardRadius;
            Vector3 randomDirectionXZ = new(randomDirection.x, 0.0f, randomDirection.y);
            player.DiscardHeldObject(placeObject: true, placePosition: targetTransform.position + randomDirectionXZ);

            Helper.CreateComponent<WaitForBehaviour>()
                  .SetPredicate(() => Vector3.Distance(jetpack.transform.position, targetTransform.position) < 5.0f)
                  .Init(() => Helper.ShortDelay(jetpack.ExplodeJetpackServerRpc));
        }

        player.carryWeight = currentWeight;
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0) {
            Chat.Print("Usage: bombard <player>");
            return;
        }

        if (localPlayer.ItemSlots.WhereIsNotNull().Count() >= 4) {
            Chat.Print("You must have an empty inventory slot!");
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
              .Init(() => BombardCommand.BombardAsync(localPlayer, targetPlayer.transform, jetpacks));
    }
}
