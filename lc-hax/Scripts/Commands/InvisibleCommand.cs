namespace Hax;

[Command("/invis")]
public class InvisibleCommand : ICommand {
    public void Execute(string[] args) {
        Setting.EnableInvisible = !Setting.EnableInvisible;
        Console.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");

        if (Setting.EnableInvisible) {
            _ = Helper.LocalPlayer?.Reflect().InvokeInternalMethod("UpdatePlayerPositionServerRpc");
        }
    }
}
