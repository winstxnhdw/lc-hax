using Hax;

[Command("/su")]
public class SuCommand : ICommand {
    public void Execute(StringArray args) {
        Setting.IsSuperuser = !Setting.IsSuperuser;
        Chat.Print($"Superuser mode is {(Setting.IsSuperuser ? "enabled" : "disabled")}!");
    }
}
