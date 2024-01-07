using Hax;

[Command("/stunclick")]
public class StunOnClickCommand : ICommand {
    public void Execute(string[] _) {
        Setting.EnableStunOnLeftClick = !Setting.EnableStunOnLeftClick;
        Chat.Print($"Stunclick: {(Setting.EnableStunOnLeftClick ? "Enabled" : "Disabled")}");
    }
}
