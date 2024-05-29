using GameNetcodeStuff;
using Hax;

[Command("mask")]
internal class MaskCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        HauntedMaskItem? hauntedMaskItem;
        if (Helper.LocalPlayer?.currentlyHeldObjectServer is not HauntedMaskItem held)
        {
            if (Helper.Grabbables.First(grabbable => grabbable is HauntedMaskItem) is not HauntedMaskItem Map)
            {
                Chat.Print("No mask found!");
                return;
            }

            if (!localPlayer.GrabObject(Map))
            {
                Chat.Print("You must have an empty inventory slot!");
                return;
            }

            hauntedMaskItem = Map;
        }
        else
        {
            hauntedMaskItem = held;
        }

        var targetPlayer = args.Length is 0
            ? localPlayer
            : Helper.GetActivePlayer(args[0]);

        if (targetPlayer is null)
        {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (!args[1].TryParse(1, out ulong amount))
        {
            Chat.Print("Invalid amount!");
            return;
        }

        Helper.CreateComponent<WaitForBehaviour>()
            .SetPredicate(() => localPlayer.IsHoldingItemOfType<HauntedMaskItem>())
            .Init(() => SpawnMimicOnPlayer(targetPlayer, hauntedMaskItem, amount));
    }

    private void SpawnMimicOnPlayer(PlayerControllerB player, HauntedMaskItem mask, ulong amount = 1)
    {
        Helper.CreateComponent<TransientBehaviour>("Mask").Init(
            _ => { mask.CreateMimicServerRpc(player.isInsideFactory, player.transform.position); }, amount * 0.1f,
            0.1f);
    }
}