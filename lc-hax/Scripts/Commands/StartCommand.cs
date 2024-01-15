using System;
using Hax;

[Command("/start")]
public class StartGameCommand : ICommand {
    public void Execute(ReadOnlySpan<string> _) {
        Helper.StartOfRound?.StartGameServerRpc();
    }
}
