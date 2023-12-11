
namespace Hax;

public class GodCommand : ICommand {
    public void Execute(string[] _) {
        Settings.EnableGodMode = !Settings.EnableGodMode;
        Helper.PrintSystem($"God mode: {(Settings.EnableGodMode ? "Enabled" : "Disabled")}");
    }
}
