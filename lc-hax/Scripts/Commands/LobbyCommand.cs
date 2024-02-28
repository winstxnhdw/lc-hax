using UnityEngine;

[Command("lobby")]
class LobbyCommand : ICommand {
    public void Execute(StringArray _) {
        if (State.ConnectedLobby is not ConnectedLobby connectedLobby) return;

        GUIUtility.systemCopyBuffer = connectedLobby.SteamId.ToString();
        Chat.Print($"The host's Steam ID {GUIUtility.systemCopyBuffer} has been copied!");
    }
}
