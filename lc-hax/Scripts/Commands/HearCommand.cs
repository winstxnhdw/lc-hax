using Hax;

[Command("/hear")]
public class HearCommand : ICommand {
    public void Execute(StringArray args) {
        Setting.EnableEavesdrop = !Setting.EnableEavesdrop;
        Chat.Print($"Hear: {(Setting.EnableEavesdrop ? "on" : "off")}");
    }
}
