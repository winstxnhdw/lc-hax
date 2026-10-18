using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("bomb")]
sealed class BombCommand : ICommand, IJetpack {
    async Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not { } localPlayer) return;
        if (args.Length is 0) {
            Chat.Print("Usage: bomb <player>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not { } targetPlayer) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (this.GetAvailableJetpack() is not { } jetpack) {
            Chat.Print("A free jetpack is required to use this command!");
            return;
        }

        if (!localPlayer.GrabObject(jetpack)) {
            Chat.Print("You must have an empty inventory slot!");
            return;
        }

        await Helper.WaitUntil(
            () => jetpack == null || localPlayer.currentlyHeldObjectServer == jetpack,
            cancellationToken
        );

        if (jetpack == null) {
            return;
        }

        localPlayer.DiscardHeldObject(placeObject: true, parentObjectTo: targetPlayer.NetworkObject);
        await Task.Delay(500, cancellationToken);
        jetpack.Unfake()?.ExplodeJetpackServerRpc();
    }
}
