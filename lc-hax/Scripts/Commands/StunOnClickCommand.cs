namespace Hax;

[Command("/stunclick")]
public class StunOnClickCommand : ICommand {
    public void Execute(string[] _) {
        Setting.EnableStunOnLeftClick = !Setting.EnableStunOnLeftClick;
        Console.Print($"Stunclick: {(Setting.EnableStunOnLeftClick ? "Enabled" : "Disabled")}");
    }
}
