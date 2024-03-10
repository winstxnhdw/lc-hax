using Hax;

[Command("sink")]
class NoSinkingCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableNoSinking = !Setting.EnableNoSinking;
        Helper.SendNotification("Sinking", $"{(Setting.EnableNoSinking ? "Disabled" : "Enabled")}");
    }
}
