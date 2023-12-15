using UnityEngine;

namespace Hax;

public class LocationCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) {
            Helper.PrintSystem("Player not found!");
            return;
        }

        Vector3 currentPostion = camera.transform.position;
        Helper.PrintSystem($"{currentPostion.x:0} {currentPostion.y:0} {currentPostion.z:0}");
    }
}
