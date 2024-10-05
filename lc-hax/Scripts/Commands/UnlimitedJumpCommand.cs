using System.Threading;
using System.Threading.Tasks;

[Command("jump")]
class UnlimitedJumpCommand : ICommand {
    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        Setting.EnableUnlimitedJump = !Setting.EnableUnlimitedJump;
        Helper.SendNotification(title: "Unlimited Jump", body: Setting.EnableUnlimitedJump ? " enabled" : "disabled");
    }
}
