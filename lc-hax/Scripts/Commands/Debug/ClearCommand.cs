namespace Hax;

[DebugCommand("/clear")]
public class ClearCommand : ICommand {
    public void Execute(string[] _) {
        Helper.HUDManager?.ChatMessageHistory.Clear();
    }
}