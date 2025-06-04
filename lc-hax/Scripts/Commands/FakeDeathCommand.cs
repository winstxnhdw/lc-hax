using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("fakedeath")]
class FakeDeathCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        Setting.EnableFakeDeath = true;

        player.KillPlayerServerRpc(
            playerId: player.PlayerIndex(),
            spawnBody: false,
            bodyVelocity: Vector3.zero,
            causeOfDeath: unchecked((int)CauseOfDeath.Unknown),
            deathAnimation: 0,
            positionOffset: Vector3.zero
        );

        await Helper.WaitUntil(() => player.playersManager.shipIsLeaving, cancellationToken);
        player.KillPlayer();
    }
}
