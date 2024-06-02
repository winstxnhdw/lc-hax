using Hax;

[Command("unlimgifts")]
internal class UnlimitedGiftCommand : ICommand
{
    public void Execute(StringArray _)
    {
        Setting.EnableUnlimitedGiftBox = !Setting.EnableUnlimitedGiftBox;
        Helper.SendFlatNotification(Setting.EnableUnlimitedGiftBox
            ? "Unlimited Gift Box enabled"
            : "Unlimited Gift Box disabled");
    }
}