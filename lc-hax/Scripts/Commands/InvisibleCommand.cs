namespace Hax;

[Command("/invis")]
public class InvisibleCommand : ICommand {
    public void Execute(string[] _) {
        Setting.EnableInvisible = !Setting.EnableInvisible;
        Console.Print($"Invisible: {(Setting.EnableInvisible ? "enabled" : "disabled")}");
    }
}
