using Hax;

[Command("unlimjump")]
internal class UnlimJumpToggleCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.UnlimitedJump = !Setting.UnlimitedJump;
        Helper.SendNotification(title: "Unlimited Jump", body: Setting.UnlimitedJump ? " enabled" : "disabled");
    }
}
