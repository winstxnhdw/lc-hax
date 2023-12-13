namespace Hax;

public class StunOnClickCommand : ICommand {
    public void Execute(string[] _) {
        Settings.EnableStunOnLeftClick = !Settings.EnableStunOnLeftClick;
    }
}
