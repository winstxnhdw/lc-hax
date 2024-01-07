using Hax;

[DebugCommand("/clear")]
public class ClearCommand : ICommand {
    public void Execute(string[] _) {
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return;
        hudManager.chatText.text = "";
    }
}
