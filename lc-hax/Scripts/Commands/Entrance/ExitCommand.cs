using Hax;

[Command("/exit")]
internal class ExitCommand : ICommand {
    public void Execute(StringArray _) => Helper.LocalPlayer?.EntranceTeleport(true);
}
