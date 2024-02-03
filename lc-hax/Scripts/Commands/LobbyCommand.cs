using Hax;
using UnityEngine;

[Command("/lobby")]
public class LobbyCommand : ICommand {
    public void Execute(StringArray _) {
        Chat.Print($"Lobby ID: {Setting.ConnectedLobbyId}");
        GUIUtility.systemCopyBuffer = Setting.ConnectedLobbyId.ToString();
    }
}
