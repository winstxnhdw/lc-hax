using Hax;

[HostCommand("eject")]
class EjectCommand : ICommand {
    public void Execute(StringArray _) => Helper.StartOfRound?.ManuallyEjectPlayersServerRpc();
}
