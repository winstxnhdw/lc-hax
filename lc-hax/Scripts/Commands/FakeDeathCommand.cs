using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("fakedeath")]
class FakeDeathCommand : ICommand {
    public void Execute(StringArray args) {
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

        Helper.CreateComponent<WaitForBehaviour>("Respawn")
              .SetPredicate(() => player.playersManager.shipIsLeaving)
              .Init(player.KillPlayer);
    }
}
