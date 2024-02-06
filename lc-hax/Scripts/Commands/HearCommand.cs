using Hax;

[Command("/hear")]
internal class HearCommand : ICommand {
    public void Execute(StringArray args) {
        Setting.EnableEavesdrop = !Setting.EnableEavesdrop;
        Chat.Print($"Hear: {(Setting.EnableEavesdrop ? "on" : "off")}");
    }
}
