using Hax;

[PrivilegedCommand("gods")]
class GodsCommand : ICommand {
    public void Execute(StringArray args) => Helper.StartOfRound?.Debug_ToggleAllowDeathServerRpc();
}
