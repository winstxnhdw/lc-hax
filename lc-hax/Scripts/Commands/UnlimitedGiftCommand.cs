using Hax;

[Command("unlimgifts")]
class UnlimitedGiftCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableUnlimitedGiftBox = !Setting.EnableUnlimitedGiftBox;
        Helper.DisplayFlatHudMessage(Setting.EnableUnlimitedGiftBox ? "Unlimited Gift Box enabled" : "Unlimited Gift Box disabled");
    }
}
