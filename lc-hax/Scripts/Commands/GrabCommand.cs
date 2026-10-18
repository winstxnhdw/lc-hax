using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;
using ZLinq;

[Command("grab")]
sealed class GrabCommand : ICommand {
    bool CanGrabItem(GrabbableObject grabbableObject, Vector3 currentPlayerPosition) =>
        grabbableObject.Unfake() is { isHeld: false } and { isHeldByEnemy: false } &&
        (grabbableObject.transform.position - currentPlayerPosition).sqrMagnitude >= 20.0f * 20.0f;

    async Task<string> GrabAllItems(
        PlayerControllerB player,
        Vector3 currentPlayerPosition,
        CancellationToken cancellationToken
    ) {
        GrabbableObject[] grabbables = [
            .. Helper.Grabbables
                     .AsValueEnumerable()
                     .Where(grabbable => this.CanGrabItem(grabbable, currentPlayerPosition))
        ];

        if (grabbables.Length is 0) {
            return "Failed to find item!";
        }

        if (!grabbables.AsValueEnumerable().Any(grabbable => player.FirstEmptyItemSlot(grabbable) != -1)) {
            return "You must have an empty inventory slot!";
        }

        float currentWeight = player.carryWeight;

        foreach (GrabbableObject grabbable in grabbables) {
            if (grabbable == null) continue;
            if (!player.GrabObject(grabbable)) continue;

            int grabFrame = Time.frameCount;

            await Helper.WaitUntil(() =>
                Time.frameCount != grabFrame && (grabbable == null || player.currentlyHeldObjectServer == grabbable),
                cancellationToken
            );

            if (grabbable == null) {
                continue;
            }

            cancellationToken.ThrowIfCancellationRequested();
            player.DiscardHeldObject();
        }

        player.carryWeight = currentWeight;

        return "Successfully grabbed all items!";
    }

    async Task<string> GrabItem(
        PlayerControllerB player,
        Vector3 currentPlayerPosition,
        string itemName,
        CancellationToken cancellationToken
    ) {
        Dictionary<string, GrabbableObject> grabbableObjects =
            Helper.Grabbables?
                .AsValueEnumerable()
                .Where(grabbable => this.CanGrabItem(grabbable, currentPlayerPosition))
                .GroupBy(grabbable => grabbable.itemProperties.name.ToLower())
                .ToDictionary(
                    group => group.Key,
                    group => Enumerable.First(group)
                ) ?? [];

        if (!itemName.ToLower().FuzzyMatch(grabbableObjects.Keys, out string key)) {
            return "Failed to find item!";
        }

        GrabbableObject grabbable = grabbableObjects[key];

        if (!player.GrabObject(grabbable)) {
            return "You must have an empty inventory slot!";
        }

        int grabFrame = Time.frameCount;

        await Helper.WaitUntil(
            () =>
                Time.frameCount != grabFrame && (grabbable == null || player.currentlyHeldObjectServer == grabbable),
            cancellationToken
        );

        if (grabbable == null) {
            return "Failed to find item!";
        }

        cancellationToken.ThrowIfCancellationRequested();
        player.DiscardHeldObject();

        return $"Grabbed a {key.ToTitleCase()}!";
    }

    async Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        Vector3 currentPlayerPosition = localPlayer.transform.position;

        string message = args.Length switch {
            0 => await this.GrabAllItems(localPlayer, currentPlayerPosition, cancellationToken),
            _ => await this.GrabItem(
                localPlayer,
                currentPlayerPosition,
                string.Join(' ', args),
                cancellationToken
            )
        };

        Chat.Print(message);
    }
}
