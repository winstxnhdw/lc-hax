using System.Threading;
using System.Threading.Tasks;

[Command("block")]
sealed class BlockCommand : ICommand {
    static string BlockEnemy() {
        Setting.EnableUntargetable = !Setting.EnableUntargetable;

        return $"{(Setting.EnableUntargetable
            ? "Enemies will no longer target you!"
            : "Enemies can now target you!")}";
    }

    static string BlockRadar() {
        Setting.EnableBlockRadar = !Setting.EnableBlockRadar;

        return $"{(Setting.EnableBlockRadar
            ? "Blocking radar targets!"
            : "No longer blocking radar targets!")}";
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: block <property>");
            return;
        }

        string result = args[0] switch {
            "enemy" => BlockCommand.BlockEnemy(),
            "radar" => BlockCommand.BlockRadar(),
            _ => "Invalid property!"
        };

        Chat.Print(result);
    }
}
