using Hax;

[DebugCommand("/drop")]
public class DropCommand : ICommand {
    public void Execute(string[] _) {
        Helper.LocalPlayer?.DropAllHeldItems();
    }
}
