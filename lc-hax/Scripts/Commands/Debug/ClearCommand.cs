using System;
using Hax;

[DebugCommand("/clear")]
public class ClearCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) return;
        hudManager.chatText.text = "";
    }
}
