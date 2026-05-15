using System.Threading;
using System.Threading.Tasks;

[Command("shovel")]
sealed class ShovelCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: shovel <force=1>");
            return Task.CompletedTask;
        }

        if (!ushort.TryParse(args[0], out ushort shovelHitForce)) {
            Chat.Print("Shovel force must be a positive number!");
            return Task.CompletedTask;
        }

        State.ShovelHitForce = shovelHitForce;
        Chat.Print($"Shovel hit force is now set to {shovelHitForce}!");
        return Task.CompletedTask;
    }
}
