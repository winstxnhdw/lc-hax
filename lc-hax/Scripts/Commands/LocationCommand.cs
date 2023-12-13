using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class LocationCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB player)) {
            Helper.PrintSystem("Player not found!");
            return;
        }

        Vector3 currentPostion = player.transform.position;
        Helper.PrintSystem($"{currentPostion.x:0}, {currentPostion.y:0}, {currentPostion.z:0}");
    }
}
