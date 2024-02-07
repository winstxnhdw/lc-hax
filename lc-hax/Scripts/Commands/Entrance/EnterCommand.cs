using Hax;

[Command("/enter")]
internal class EnterCommand : ICommand {
    public void Execute(StringArray _) => Helper.LocalPlayer?.EntranceTeleport(false);
}
