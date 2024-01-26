using Hax;

[DebugCommand("/clear")]
public class ClearCommand : ICommand {
    public void Execute(StringArray _) {
        if (Helper.HUDManager is not HUDManager hudManager) return;

        hudManager.chatText.text = "";
        hudManager.ChatMessageHistory.Clear();
    }
}
