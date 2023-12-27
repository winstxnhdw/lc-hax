namespace Hax;

public class StunOnClickCommand : ICommand {
    public void Execute(string[] _) {
        Setting.EnableStunOnLeftClick = !Setting.EnableStunOnLeftClick;
    }
}
