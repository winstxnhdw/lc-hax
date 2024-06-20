#region

using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;
using Object = UnityEngine.Object;

#endregion

[Command("respawn")]
class RespawnCommand : ICommand {
    public void Execute(StringArray _) => this.RespawnLocalPlayer();

    void RespawnLocalPlayer() {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;
        Helper.RespawnLocalPlayer();
        Helper.CreateComponent<WaitForBehaviour>("Respawn")
            .SetPredicate(() => hudManager.localPlayer.playersManager.shipIsLeaving)
            .Init(localPlayer.KillPlayer);
        Helper.SendFlatNotification("You got Respawned Locally (You will die once ship leaves)");

    }


}
