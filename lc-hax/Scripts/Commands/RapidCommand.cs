namespace Hax;

[Command("/rapid")]
public class RapidCommand : ICommand {
    public void Execute(string[] _) {
        Setting.EnableNoCooldown = !Setting.EnableNoCooldown;
        Console.Print($"Rapid fire: {(Setting.EnableNoCooldown ? "Enabled" : "Disabled")}");
    }
}
