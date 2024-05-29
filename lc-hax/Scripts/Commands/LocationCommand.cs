using Hax;
using UnityEngine;

[Command("xyz")]
internal class LocationCommand : ICommand
{
    public void Execute(StringArray _)
    {
        if (Helper.CurrentCamera is not Camera camera) return;

        var currentPostion = camera.transform.position;
        Chat.Print($"{currentPostion.x:0} {currentPostion.y:0} {currentPostion.z:0}");
    }
}