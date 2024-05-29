using Hax;

[Command("jump")]
internal class UnlimitedJumpCommand : ICommand
{
    public void Execute(StringArray _)
    {
        Setting.EnableUnlimitedJump = !Setting.EnableUnlimitedJump;
        Helper.SendNotification("Unlimited Jump", Setting.EnableUnlimitedJump ? " enabled" : "disabled");
    }
}