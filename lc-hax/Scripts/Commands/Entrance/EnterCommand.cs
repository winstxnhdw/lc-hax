using Hax;

[Command("enter")]
class EnterCommand : ICommand {
    public void Execute(StringArray _) => Helper.LocalPlayer?.EntranceTeleport(false);
}
