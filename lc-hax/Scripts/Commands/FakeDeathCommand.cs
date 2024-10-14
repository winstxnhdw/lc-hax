using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("fakedeath")]
class FakeDeathCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        Setting.EnableFakeDeath = true;

        _ = player.Reflect().InvokeInternalMethod(
            "KillPlayerServerRpc",
            player.PlayerIndex(),
            false,
            Vector3.zero,
            CauseOfDeath.Unknown,
            0
        );

        await Helper.WaitUntil(() => player.playersManager.shipIsLeaving, cancellationToken);
        player.KillPlayer();
    }
}
