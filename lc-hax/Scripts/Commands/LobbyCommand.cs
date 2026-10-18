using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("lobby")]
sealed class LobbyCommand : ICommand {
    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (State.ConnectedLobby is not { } connectedLobby) return Task.CompletedTask;
        GUIUtility.systemCopyBuffer = connectedLobby.Lobby.Owner.Id.ToString();
        Chat.Print($"The host's Steam ID {GUIUtility.systemCopyBuffer} has been copied!");
        return Task.CompletedTask;
    }
}
