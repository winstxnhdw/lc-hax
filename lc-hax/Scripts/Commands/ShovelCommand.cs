using System.Threading;
using System.Threading.Tasks;

[Command("shovel")]
class ShovelCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: shovel <force=1>");
            return;
        }

        if (!ushort.TryParse(args[0], out ushort shovelHitForce)) {
            Chat.Print("Invalid value!");
            return;
        }

        State.ShovelHitForce = shovelHitForce;
        Chat.Print($"Shovel hit force is now set to {shovelHitForce}!");
    }
}
