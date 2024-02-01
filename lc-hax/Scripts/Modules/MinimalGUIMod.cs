using UnityEngine;
using Hax;

public class MinimalGUIMod : MonoBehaviour {
    bool InGame { get; set; } = false;
    bool ShowLobbyInfo { get; set; } = true;

    void OnEnable() {
        GameListener.onGameStart += this.ToggleInGame;
        GameListener.onGameEnd += this.ToggleNotInGame;
        InputListener.onF9Press += LobbyInfo;
    }

    void OnDisable() {
        GameListener.onGameStart -= this.ToggleInGame;
        GameListener.onGameEnd -= this.ToggleNotInGame;
        InputListener.onF9Press -= LobbyInfo;
    }

    void OnGUI() {
        GUIStyle labelStyle = GUI.skin.label;
        string lobbyIdText = "";

        if (ShowLobbyInfo) {
            lobbyIdText = $"Lobby ID: <color=#0EE600>{(Setting.ConnectedLobbyId)}</color><color=#999999>" +
                    $"\nShift F4: Disconnect" +
                    $"\nShift F3: Disconnect + ReJoin" +
                    $"\nF9: Toggle Lobby Info</color>";
        }
        if (ShowLobbyInfo && !this.InGame) {
            lobbyIdText = $"Lobby ID: <color=#0EE600>{(Setting.ConnectedLobbyId)}</color><color=#999999>" +
                $"\nShift F3: Join ID" +
                $"\nShift C: Copy ID" +
                $"\nShift V: Paste Clipboard To ID" +
                $"\nF9: Toggle Lobby Info</color>";
        }
        string antiKickText = $"Anti-Kick: {(Setting.EnableAntiKick ? "<color=#0EE600>On</color>" : "<color=#E60000>Off</color>")}";

        Vector2 lobbyIdSize = labelStyle.CalcSize(new GUIContent(lobbyIdText));
        float lobbyIdXPosition = 10f;
        float lobbyIdYPosition = 0;
        Rect lobbyIdRect = new Rect(lobbyIdXPosition, lobbyIdYPosition, lobbyIdSize.x, lobbyIdSize.y);

        Vector2 antiKickSize = labelStyle.CalcSize(new GUIContent(antiKickText));
        float antiKickXPosition = Screen.width - antiKickSize.x - 10;
        float antiKickYPosition = 0;
        Rect antiKickRect = new Rect(antiKickXPosition, antiKickYPosition, antiKickSize.x, antiKickSize.y);

        GUI.Label(lobbyIdRect, lobbyIdText);

        if (!this.InGame) {
            GUI.Label(antiKickRect, antiKickText);
        }
    }

    void LobbyInfo() {
        ShowLobbyInfo = !ShowLobbyInfo;
    }

    void ToggleInGame() => this.InGame = true;

    void ToggleNotInGame() => this.InGame = false;
}
