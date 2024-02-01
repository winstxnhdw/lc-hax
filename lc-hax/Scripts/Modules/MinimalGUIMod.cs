using UnityEngine;
using Hax;

public class MinimalGUIMod : MonoBehaviour {
    bool InGame { get; set; } = false;

    void OnEnable() {
        GameListener.onGameStart += this.ToggleInGame;
        GameListener.onGameEnd += this.ToggleNotInGame;
    }

    void OnDisable() {
        GameListener.onGameStart -= this.ToggleInGame;
        GameListener.onGameEnd -= this.ToggleNotInGame;
    }

    void OnGUI() {
        GUIStyle labelStyle = GUI.skin.label;
        string lobbyIdText = $"Lobby Id: {Setting.ConnectedLobbyId}";
        string antiKickText = $"Anti-Kick: {(Setting.EnableAntiKick ? "On" : "Off")}";

        Vector2 lobbyIdSize = labelStyle.CalcSize(new GUIContent(lobbyIdText));
        float lobbyIdXPosition = Screen.width - lobbyIdSize.x - 10;
        float lobbyIdYPosition = 0;
        Rect lobbyIdRect = new(lobbyIdXPosition, lobbyIdYPosition, lobbyIdSize.x, lobbyIdSize.y);

        Vector2 antiKickSize = labelStyle.CalcSize(new GUIContent(antiKickText));
        float antiKickXPosition = Screen.width - antiKickSize.x - 10;
        float antiKickYPosition = lobbyIdYPosition + lobbyIdSize.y;
        Rect antiKickRect = new(antiKickXPosition, antiKickYPosition, antiKickSize.x, antiKickSize.y);

        GUI.Label(lobbyIdRect, lobbyIdText);

        if (!this.InGame) {
            GUI.Label(antiKickRect, antiKickText);
        }
    }

    void ToggleInGame() => this.InGame = true;

    void ToggleNotInGame() => this.InGame = false;
}
