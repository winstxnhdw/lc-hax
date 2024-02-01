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
        string labelText;
        if (!this.InGame) {
            labelText = $"Lobby Id: {(Setting.ConnectedLobbyId)}" +
                $"\nAnti-Kick: {(Setting.EnableAntiKick ? "On" : "Off")}";
        }
        else {
            labelText = $"Lobby Id: {(Setting.ConnectedLobbyId)}";
        }
        GUIStyle labelStyle = GUI.skin.label;
        Vector2 labelSize = labelStyle.CalcSize(new GUIContent(labelText));
        float xPosition = Screen.width - labelSize.x - 10;
        float yPosition = 0;
        Rect labelRect = new(xPosition, yPosition, labelSize.x, labelSize.y);
        GUI.Label(labelRect, labelText);
    }

    void ToggleInGame() => this.InGame = true;

    void ToggleNotInGame() => this.InGame = false;
}
