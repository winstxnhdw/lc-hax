#region

using Hax;

#endregion

[Command("exit")]
class ExitCommand : ICommand {
    public void Execute(StringArray _) => Helper.LocalPlayer?.EntranceTeleport(true);
}
