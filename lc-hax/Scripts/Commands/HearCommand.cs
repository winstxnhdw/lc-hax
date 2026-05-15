using System.Threading;
using System.Threading.Tasks;

[Command("hear")]
sealed class HearCommand : ICommand {
    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        Setting.EnableEavesdrop = !Setting.EnableEavesdrop;
        Chat.Print($"Hear: {(Setting.EnableEavesdrop ? "on" : "off")}");
        return Task.CompletedTask;
    }
}
