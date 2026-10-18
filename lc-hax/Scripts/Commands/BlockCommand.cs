using System.Threading;
using System.Threading.Tasks;

[Command("block")]
sealed class BlockCommand : ICommand {
    string BlockEnemy() {
        Globals.EnableUntargetable = !Globals.EnableUntargetable;

        return $"{(Globals.EnableUntargetable
            ? "Enemies will no longer target you!"
            : "Enemies can now target you!")}";
    }

    string BlockRadar() {
        Globals.EnableBlockRadar = !Globals.EnableBlockRadar;

        return $"{(Globals.EnableBlockRadar
            ? "Blocking radar targets!"
            : "No longer blocking radar targets!")}";
    }

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: block <property>");
            return Task.CompletedTask;
        }

        string result = args[0] switch {
            "enemy" => this.BlockEnemy(),
            "radar" => this.BlockRadar(),
            _ => "Invalid property!"
        };

        Chat.Print(result);
        return Task.CompletedTask;
    }
}
