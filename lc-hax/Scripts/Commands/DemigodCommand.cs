namespace Hax;

public class DemiGodCommand : ICommand {
    public void Execute(string[] _) {
        Settings.EnableDemigodMode = !Settings.EnableDemigodMode;
        Console.Print($"Demigod mode: {(Settings.EnableDemigodMode ? "Enabled" : "Disabled")}");
    }
}
