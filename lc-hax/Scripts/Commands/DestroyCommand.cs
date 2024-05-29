using System.Collections;
using System.Linq;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("destroy")]
internal class DestroyCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        if (!player.HasFreeSlots())
        {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        var result = args[0] switch
        {
            null => DestroyHeldItem(player),
            "--all" => DestroyAllItems(player),
            _ => new Result(message: "Invalid arguments!")
        };

        if (!result.Success) Chat.Print(result.Message);
    }

    private IEnumerator DestroyAllItemsAsync(PlayerControllerB player)
    {
        var currentWeight = player.carryWeight;

        foreach (var grabbable in Helper.Grabbables.ToArray())
        {
            if (!player.GrabObject(grabbable)) continue;
            yield return new WaitUntil(() => player.IsHoldingGrabbable(grabbable));
            Helper.RemoveItemFromHud(player.GetSlotOfItem(grabbable));
            player.DespawnHeldObject();
        }

        player.carryWeight = currentWeight;
    }

    private Result DestroyHeldItem(PlayerControllerB player)
    {
        if (player.currentlyHeldObjectServer is null) return new Result(message: "You are not holding anything!");

        Helper.RemoveItemFromHud(player.GetSlotOfItem(player.currentlyHeldObjectServer));
        player.DespawnHeldObject();
        return new Result(true);
    }

    private Result DestroyAllItems(PlayerControllerB player)
    {
        Helper.CreateComponent<AsyncBehaviour>()
            .Init(() => DestroyAllItemsAsync(player));

        return new Result(true);
    }
}