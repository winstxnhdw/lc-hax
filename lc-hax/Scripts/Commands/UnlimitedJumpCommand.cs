using System.Threading;
using System.Threading.Tasks;

[Command("jump")]
sealed class UnlimitedJumpCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        Globals.EnableUnlimitedJump = !Globals.EnableUnlimitedJump;
        Helper.SendNotification(title: "Unlimited Jump", body: Globals.EnableUnlimitedJump ? " enabled" : "disabled");
        return Task.CompletedTask;
    }
}
