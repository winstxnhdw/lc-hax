using Hax;

[Command("unlimgifts")]
class UnlimitedGiftCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableUnlimitedJump = !Setting.EnableUnlimitedGiftBox;
        Helper.SendNotification(title: "Unlimited Gift Box", body: Setting.EnableUnlimitedGiftBox ? " enabled" : "disabled");
    }
}
