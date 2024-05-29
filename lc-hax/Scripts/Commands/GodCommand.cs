[Command("god")]
internal class GodCommand : ICommand
{
    public void Execute(StringArray _)
    {
        Setting.EnableGodMode = !Setting.EnableGodMode;
        Chat.Print($"God mode: {(Setting.EnableGodMode ? "Enabled" : "Disabled")}");
    }
}