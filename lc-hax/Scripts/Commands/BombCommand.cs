using System;
using Hax;

[Command("bomb")]
internal class BombCommand : ICommand {
    JetpackItem? GetAvailableJetpack() =>
        Helper.FindObjects<JetpackItem>()
              .First(jetpack => !jetpack.Reflect().GetInternalField<bool>("jetpackBroken"));

    Action BlowUpLocation(PlayerControllerB localPlayer, PlayerControllerB targetPlayer, JetpackItem jetpack) => () => {
        localPlayer.DiscardHeldObject(placeObject: true, parentObjectTo: targetPlayer.NetworkObject);
        Helper.ShortDelay(jetpack.ExplodeJetpackServerRpc);
    };

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0) {
            Chat.Print("Usage: bomb <player>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (this.GetAvailableJetpack() is not JetpackItem jetpack) {
            Chat.Print("A free jetpack is required to use this command!");
            return;
        }

        if (!localPlayer.GrabObject(jetpack)) {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        Helper.CreateComponent<WaitForBehaviour>("Throw Bomb")
              .SetPredicate(() => localPlayer.ItemSlots[localPlayer.currentItemSlot] == jetpack)
              .Init(this.BlowUpLocation(localPlayer, targetPlayer, jetpack));
    }
}
