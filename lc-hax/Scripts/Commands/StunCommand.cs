using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("stun")]
class StunCommand : ICommand, IStun {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.CurrentCamera is not Camera camera) return;
        if (args.Length is 0) {
            Chat.Print("Usage: stun <duration>");
            return;
        }

        if (!ulong.TryParse(args[0], out ulong duration)) {
            Chat.Print($"Stun {nameof(duration)} must be a positive number!");
            return;
        }

        this.Stun(camera.transform.position, float.MaxValue, duration);
    }
}
