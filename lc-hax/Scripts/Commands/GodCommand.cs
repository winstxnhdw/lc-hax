namespace Hax;

public class GodCommand : ICommand {
    public void Execute(string[] _) {
        Settings.EnableGodMode = !Settings.EnableGodMode;
        Console.Print($"God mode: {(Settings.EnableGodMode ? "Enabled" : "Disabled")}");
    }
}
