using System.Threading;
using System.Threading.Tasks;

[Command("jump")]
sealed class UnlimitedJumpCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        Setting.EnableUnlimitedJump = !Setting.EnableUnlimitedJump;
        Helper.SendNotification(title: "Unlimited Jump", body: Setting.EnableUnlimitedJump ? " enabled" : "disabled");
    }
}
