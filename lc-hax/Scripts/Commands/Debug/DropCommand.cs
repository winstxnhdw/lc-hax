using Hax;

[DebugCommand("/drop")]
public class DropCommand : ICommand {
    public void Execute(StringArray _) {
        Helper.LocalPlayer?.DropAllHeldItems();
    }
}
