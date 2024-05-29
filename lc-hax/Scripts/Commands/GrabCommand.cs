using System.Collections;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("grab")]
internal class GrabCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (!localPlayer.HasFreeSlots())
        {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        var currentPlayerPosition = localPlayer.transform.position;

        var message = args.Length switch
        {
            0 => GrabAllItems(localPlayer, currentPlayerPosition),
            _ => GrabItem(localPlayer, currentPlayerPosition, string.Join(' ', args))
        };

        Chat.Print(message);
    }

    private bool CanGrabItem(GrabbableObject grabbableObject, Vector3 currentPlayerPosition)
    {
        return grabbableObject is { isHeld: false } and { isHeldByEnemy: false } &&
               (grabbableObject.transform.position - currentPlayerPosition).sqrMagnitude >= 20.0f * 20.0f;
    }

    private IEnumerator GrabAllItemsAsync(PlayerControllerB player, GrabbableObject[] grabbables)
    {
        var currentWeight = player.carryWeight;

        foreach (var grabbable in grabbables)
        {
            if (!player.GrabObject(grabbable)) continue;
            yield return new WaitUntil(() => player.IsHoldingGrabbable(grabbable));
            player.DiscardObject(grabbable);
        }

        player.carryWeight = currentWeight;
    }

    private string GrabAllItems(PlayerControllerB player, Vector3 currentPlayerPosition)
    {
        var grabbables =
            Helper.Grabbables
                .WhereIsNotNull()
                .Where(grabbable => CanGrabItem(grabbable, currentPlayerPosition))
                .ToArray();

        Helper.CreateComponent<AsyncBehaviour>()
            .Init(() => GrabAllItemsAsync(player, grabbables));

        return "Successfully grabbed all items!";
    }

    private string GrabItem(PlayerControllerB player, Vector3 currentPlayerPosition, string itemName)
    {
        var grabbableObjects =
            Helper.Grabbables?
                .WhereIsNotNull()
                .Where(grabbable => CanGrabItem(grabbable, currentPlayerPosition))
                .GroupBy(grabbable => grabbable.itemProperties.name.ToLower())
                .ToDictionary(
                    group => group.Key,
                    group => Enumerable.First(group)
                ) ?? [];

        if (!itemName.ToLower().FuzzyMatch(grabbableObjects.Keys, out var key)) return "Failed to find item!";

        var grabbable = grabbableObjects[key];

        if (!player.GrabObject(grabbable)) return "You must have an empty inventory slot!";

        Helper.CreateComponent<WaitForBehaviour>()
            .SetPredicate(() => player.IsHoldingGrabbable(grabbable))
            .Init(() => { player.DiscardObject(grabbable); });

        return $"Grabbed a {key.ToTitleCase()}!";
    }
}