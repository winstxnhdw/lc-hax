using Hax;

[PrivilegedCommand("eject")]
class EjectCommand : ICommand {
    public void Execute(StringArray _) => Helper.StartOfRound?.ManuallyEjectPlayersServerRpc();
}
