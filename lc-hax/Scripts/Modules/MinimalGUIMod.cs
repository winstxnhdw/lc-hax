using UnityEngine;

internal sealed class MinimalGUIMod : MonoBehaviour
{
    private bool InGame { get; set; } = false;

    private void OnEnable()
    {
        GameListener.OnGameStart += ToggleInGame;
        GameListener.OnGameEnd += ToggleNotInGame;
    }

    private void OnDisable()
    {
        GameListener.OnGameStart -= ToggleInGame;
        GameListener.OnGameEnd -= ToggleNotInGame;
    }

    private void OnGUI()
    {
        if (InGame) return;

        var labelText = $"Anti-Kick: {(Setting.EnableAntiKick ? "On" : "Off")}";
        var labelStyle = GUI.skin.label;
        var labelSize = labelStyle.CalcSize(new GUIContent(labelText));
        var xPosition = Screen.width - labelSize.x - 10;
        float yPosition = 0;
        Rect labelRect = new(xPosition, yPosition, labelSize.x, labelSize.y);
        GUI.Label(labelRect, labelText);
    }

    private void ToggleInGame()
    {
        InGame = true;
    }

    private void ToggleNotInGame()
    {
        InGame = false;
    }
}