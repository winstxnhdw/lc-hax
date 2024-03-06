using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("drop")]
class DropCommand : ICommand {
    public void Execute(StringArray _) {
        if(Helper.LocalPlayer is not PlayerControllerB player) return;
        player.DropAllHeldItemsAndSync();
    }
}
