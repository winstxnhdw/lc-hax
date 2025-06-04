using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Command("lobby")]
class LobbyCommand : ICommand {
    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (State.ConnectedLobby is not ConnectedLobby connectedLobby) return;

        GUIUtility.systemCopyBuffer = connectedLobby.Lobby.Owner.Id.ToString();
        Chat.Print($"The host's Steam ID {GUIUtility.systemCopyBuffer} has been copied!");
    }
}
