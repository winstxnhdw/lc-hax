using Hax;

[HostCommand("revive")]
class ReviveCommand : ICommand {
    public void Execute(StringArray args) => Helper.StartOfRound?.Debug_ReviveAllPlayersServerRpc();
}
