[Command("killclick")]
class KillClickCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.EnableKillOnLeftClick = !Setting.EnableKillOnLeftClick;
        Chat.Print($"Killclick: {(Setting.EnableKillOnLeftClick ? "Enabled" : "Disabled")}");
    }
}
