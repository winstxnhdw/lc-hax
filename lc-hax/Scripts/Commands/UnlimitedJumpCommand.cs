#region

using Hax;

#endregion

[Command("jump")]
class UnlimitedJumpCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableUnlimitedJump = !Setting.EnableUnlimitedJump;
        Helper.SendFlatNotification(
            Setting.EnableUnlimitedJump ? "Unlimited Jump: enabled" : "Unlimited Jump: disabled");
    }
}
