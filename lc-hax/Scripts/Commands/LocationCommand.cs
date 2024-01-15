using System;
using UnityEngine;
using Hax;

[Command("/xyz")]
public class LocationCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) {
            Chat.Print("Player not found!");
            return;
        }

        Vector3 currentPostion = camera.transform.position;
        Chat.Print($"{currentPostion.x:0} {currentPostion.y:0} {currentPostion.z:0}");
    }
}
