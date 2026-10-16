using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("stun")]
sealed class StunCommand : ICommand, IStun {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.CurrentCamera is not Camera camera)
            return Task.CompletedTask;
        if (args.Length is 0) {
            Chat.Print("Usage: stun <duration>");
            return Task.CompletedTask;
        }

        if (!ulong.TryParse(args[0], out ulong duration)) {
            Chat.Print($"Stun {nameof(duration)} must be a positive number!");
            return Task.CompletedTask;
        }

        this.Stun(camera.transform.position, float.MaxValue, duration);
        return Task.CompletedTask;
    }
}
