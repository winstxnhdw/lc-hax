using UnityEngine;

sealed class MinimalGUIMod : MonoBehaviour {
    bool InGame { get; set; }

    void OnEnable() {
        GameListener.OnGameStart += this.ToggleInGame;
        GameListener.OnGameEnd += this.ToggleNotInGame;
    }

    void OnDisable() {
        GameListener.OnGameStart -= this.ToggleInGame;
        GameListener.OnGameEnd -= this.ToggleNotInGame;
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
