using Hax;

[Command("unlimjump")]
internal class UnlimJumpToggleCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.UnlimitedJump = !Setting.UnlimitedJump;
        Helper.SendNotification("Unlimited Jump", Setting.UnlimitedJump ? " enabled" : "disabled");
    }
}
