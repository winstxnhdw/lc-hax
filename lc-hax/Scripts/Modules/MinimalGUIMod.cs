using UnityEngine;
using Hax;

internal class MinimalGUIMod : MonoBehaviour {
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
        if (this.InGame) return;

        string labelText = $"Anti-Kick: {(Setting.EnableAntiKick ? "On" : "Off")}";
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
