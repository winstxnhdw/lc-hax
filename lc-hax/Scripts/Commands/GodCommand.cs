using System.Threading;
using System.Threading.Tasks;

[Command("god")]
sealed class GodCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Globals.EnableGodMode = !Globals.EnableGodMode;
        Chat.Print($"God mode: {(Globals.EnableGodMode ? "Enabled" : "Disabled")}");
        return Task.CompletedTask;
    }
}
