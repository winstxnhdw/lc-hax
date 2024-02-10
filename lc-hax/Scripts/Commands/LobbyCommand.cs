using Hax;
using UnityEngine;

[Command("lobby")]
internal class LobbyCommand : ICommand {
    public void Execute(StringArray _) {
        Chat.Print($"The lobby ID {State.ConnectedLobbyId} has been copied!");
        GUIUtility.systemCopyBuffer = State.ConnectedLobbyId.ToString();
    }
}
