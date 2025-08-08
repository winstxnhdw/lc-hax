using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("xyz")]
sealed class LocationCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.CurrentCamera is not Camera camera) return;

        Vector3 currentPostion = camera.transform.position;
        Chat.Print($"{currentPostion.x:0} {currentPostion.y:0} {currentPostion.z:0}");
    }
}
