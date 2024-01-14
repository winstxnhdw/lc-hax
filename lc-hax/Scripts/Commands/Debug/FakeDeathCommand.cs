using GameNetcodeStuff;
using UnityEngine;
using Hax;
[DebugCommand("/fakedeath")]
public class FakeDeathCommand : ICommand {
    public void Execute(string[] args) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer)) {
            return;
        }

        //call only once, when round starts, to garuntee no more players are joining,
        //you will announce to other clients you died without leaving body behind.

        //the host will register you death as 1 death, so becareful,
        //or else ship can leave thinking everyone died

        //you will revive in everyone's world when the round ends. 
        //just don't move to not update your position to other clients
        _ = localPlayer.Reflect().InvokeInternalMethod(
            "KillPlayerServerRpc",
            (int)localPlayer.playerClientId,
            false,
            Vector3.zero,
            0,
            0);
    }
}
