using System;
using Hax;

[DebugCommand("/drop")]
public class DropCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        Helper.LocalPlayer?.DropAllHeldItems();
    }
}
