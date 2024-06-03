#region

using Hax;

#endregion

[Command("enter")]
class EnterCommand : ICommand {
    public void Execute(StringArray _) => Helper.LocalPlayer?.EntranceTeleport(false);
}
