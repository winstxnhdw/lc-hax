using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("bombard")]
internal class BombardCommand : ICommand, IJetpack
{
    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0)
        {
            Chat.Print("Usage: bombard <player>");
            return;
        }

        if (!localPlayer.HasFreeSlots())
        {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer)
        {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        var jetpacks = this.GetAvailableJetpacks();

        if (jetpacks.Length is 0)
        {
            Chat.Print("A usable jetpack is required to use this command!");
            return;
        }

        Helper.CreateComponent<AsyncBehaviour>()
            .Init(() => BombardAsync(localPlayer, targetPlayer.transform, jetpacks));
    }

    /// <summary>
    ///     Grab and discard jetpacks to a random location of the same elevation near the target player.
    ///     If the target player is far away, it may take a while for the jetpacks to reach the player.
    ///     The jetpacks will only explode if they within 5 units of the target player.
    /// </summary>
    private IEnumerator BombardAsync(PlayerControllerB player, Transform targetTransform, JetpackItem[] jetpacks)
    {
        var currentWeight = player.carryWeight;

        foreach (var jetpack in jetpacks)
        {
            if (!player.GrabObject(jetpack)) continue;
            yield return new WaitUntil(() => player.IsHoldingGrabbable(jetpack));

            const float bombardRadius = 10.0f;
            var randomDirection = Random.insideUnitCircle * bombardRadius;
            Vector3 randomDirectionXZ = new(randomDirection.x, 0.0f, randomDirection.y);
            player.DiscardObject(jetpack, true, placePosition: targetTransform.position + randomDirectionXZ);

            Helper.CreateComponent<WaitForBehaviour>()
                .SetPredicate(() => Vector3.Distance(jetpack.transform.position, targetTransform.position) < 5.0f)
                .Init(() => Helper.ShortDelay(jetpack.ExplodeJetpackServerRpc));
        }

        player.carryWeight = currentWeight;
    }
}