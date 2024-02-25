using Hax;

[Command("exit")]
class ExitCommand : ICommand {
    public void Execute(StringArray _) => Helper.LocalPlayer?.EntranceTeleport(true);
}
