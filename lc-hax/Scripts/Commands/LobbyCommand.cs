using Hax;
using UnityEngine;

[Command("/lobby")]
public class LobbyCommand : ICommand {
    public void Execute(StringArray _) {
        Chat.Print($"The lobby ID {Setting.ConnectedLobbyId} has been copied!");
        GUIUtility.systemCopyBuffer = Setting.ConnectedLobbyId.ToString();
    }
}
