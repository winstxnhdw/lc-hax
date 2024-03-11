using GameNetcodeStuff;
using Hax;
using UnityEngine;

[DebugCommand("fixinventory")]
class FixInventoryCommand : ICommand {
    public void Execute(StringArray _) {
        if(Helper.LocalPlayer is not PlayerControllerB player) return;
        Helper.Grabbables.ForEach(grabbable => {
            if(grabbable is not null) {
                if(grabbable.playerHeldBy == player) {
                    grabbable.Detach();
                }
                else if(grabbable.parentObject == player.localItemHolder) {
                    grabbable.Detach();
                }
                else if (grabbable.parentObject == player.serverItemHolder) {
                    grabbable.Detach();
                }
            }
        });
    }
}
