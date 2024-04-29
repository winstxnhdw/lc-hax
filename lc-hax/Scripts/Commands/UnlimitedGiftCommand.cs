using Hax;

[Command("unlimgifts")]
class UnlimitedGiftCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableUnlimitedGiftBox = !Setting.EnableUnlimitedGiftBox;
        Helper.SendNotification(title: "Unlimited Gift Box", body: Setting.EnableUnlimitedGiftBox ? " enabled" : "disabled");
    }
}
