using Hax;

[HostCommand("eject")]
internal class EjectCommand : ICommand
{
    public void Execute(StringArray _)
    {
        Helper.StartOfRound?.ManuallyEjectPlayersServerRpc();
    }
}