using Hax;

[Command("/rapid")]
internal class RapidCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableNoCooldown = !Setting.EnableNoCooldown;
        Chat.Print($"Rapid fire: {(Setting.EnableNoCooldown ? "Enabled" : "Disabled")}");
    }
}
