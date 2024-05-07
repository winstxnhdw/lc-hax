using Hax;

[Command("sink")]
class NoSinkingCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableNoSinking = !Setting.EnableNoSinking;
        Helper.DisplayFlatHudMessage(Setting.EnableNoSinking ? "Sinking Disabled" : "Sinking Enabled");
    }
}
