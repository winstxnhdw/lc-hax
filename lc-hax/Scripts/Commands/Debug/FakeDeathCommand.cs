using GameNetcodeStuff;
using UnityEngine;
using Hax;

[DebugCommand("/fakedeath")]
internal class FakeDeathCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        //CAN COMBO WITH ANTI-KICK AND /INVIS
        // YOU MUST CALL EVERY START OF ROUND

        //call only once, when round starts, to garuntee no more players are joining,
        //you will announce to other clients you died without leaving body behind.

        //the host will register you death as 1 death, so becareful,
        //or else ship can leave thinking everyone died

        //you will revive in everyone's world when the round ends.
        //just don't move to not update your position to other clients

        Setting.EnableFakeDeath = true;

        _ = player.Reflect().InvokeInternalMethod(
            "KillPlayerServerRpc",
            player.PlayerIndex(),
            false,
            Vector3.zero,
            CauseOfDeath.Unknown,
            0
        );
    }
}
