using Hax;

[Command("stunclick")]
class StunOnClickCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableStunOnLeftClick = !Setting.EnableStunOnLeftClick;
        Chat.Print($"Stunclick: {(Setting.EnableStunOnLeftClick ? "Enabled" : "Disabled")}");
    }
}
