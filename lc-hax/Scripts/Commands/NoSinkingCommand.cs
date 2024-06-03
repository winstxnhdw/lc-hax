#region

using Hax;

#endregion

[Command("sink")]
class NoSinkingCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableNoSinking = !Setting.EnableNoSinking;
        Helper.SendFlatNotification(Setting.EnableNoSinking ? "Sinking Disabled" : "Sinking Enabled");
    }
}
