namespace Hax;

public class GodCommand : ICommand {
    public void Execute(string[] _) {
        Setting.EnableGodMode = !Setting.EnableGodMode;
        Console.Print($"God mode: {(Setting.EnableGodMode ? "Enabled" : "Disabled")}");
    }
}
