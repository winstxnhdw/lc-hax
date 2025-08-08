using System.Threading;
using System.Threading.Tasks;

[Command("shovel")]
sealed class ShovelCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: shovel <force=1>");
            return;
        }

        if (!ushort.TryParse(args[0], out ushort shovelHitForce)) {
            Chat.Print("Shovel force must be a positive number!");
            return;
        }

        State.ShovelHitForce = shovelHitForce;
        Chat.Print($"Shovel hit force is now set to {shovelHitForce}!");
    }
}
