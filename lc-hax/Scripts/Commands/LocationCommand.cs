using Hax;
using UnityEngine;

[Command("xyz")]
class LocationCommand : ICommand {
    public void Execute(StringArray _) {
        if (Helper.CurrentCamera is not Camera camera) return;

        Vector3 currentPostion = camera.transform.position;
        Chat.Print($"{currentPostion.x:0} {currentPostion.y:0} {currentPostion.z:0}");
    }
}
