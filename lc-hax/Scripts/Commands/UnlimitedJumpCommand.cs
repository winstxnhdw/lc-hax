using Hax;

[Command("jump")]
class UnlimitedJumpCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableUnlimitedJump = !Setting.EnableUnlimitedJump;
        Helper.SendNotification(title: "Unlimited Jump", body: Setting.EnableUnlimitedJump ? " enabled" : "disabled");
    }
}
