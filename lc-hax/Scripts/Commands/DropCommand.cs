using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("drop")]
class DropCommand : ICommand {
    public void Execute(StringArray _) {
        if(Helper.LocalPlayer is not PlayerControllerB player) return;
        player.DropAllHeldItemsAndSync();
        // get all exisitng items
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
