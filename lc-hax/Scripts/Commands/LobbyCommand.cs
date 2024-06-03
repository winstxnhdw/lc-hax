#region

using UnityEngine;

#endregion

[Command("lobby")]
class LobbyCommand : ICommand {
    public void Execute(StringArray _) {
        if (State.ConnectedLobby is not ConnectedLobby connectedLobby) return;

        GUIUtility.systemCopyBuffer = connectedLobby.Lobby.Owner.Id.ToString();
        Chat.Print($"The host's Steam ID {GUIUtility.systemCopyBuffer} has been copied!");
    }
}
