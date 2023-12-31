namespace Hax;

[Command("/demigod")]
public class DemiGodCommand : ICommand {
    public void Execute(string[] _) {
        Setting.EnableDemigodMode = !Setting.EnableDemigodMode;
        Console.Print($"Demigod mode: {(Setting.EnableDemigodMode ? "Enabled" : "Disabled")}");
    }
}
