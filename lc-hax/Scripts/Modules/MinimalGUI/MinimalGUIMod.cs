using System.Linq;
using hax;
using UnityEngine;

sealed class MinimalGUIMod : MonoBehaviour {
    GUIStyle labelStyle;
    internal static MinimalGUIMod? Instance { get; private set; }

    void Awake() {
        if (Instance != null) Destroy(this);
        Instance = this;
    }

    void OnGUI() {
        if (!this.enabled) return;

        float yPosition = 0;

        // Create labelStyle if it's null
        if (labelStyle == null) {
            labelStyle = new GUIStyle(GUI.skin.label) {
                fontSize = 25,
                wordWrap = true
            };
        }

        // Accessing Texts dictionary from MinimalGUIHelper
        var texts = MinimalGUIHelper.Texts;

        // Sort texts by their index
        texts = texts.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);

        foreach (var kvp in texts)
        foreach (var innerKvp in kvp.Value) {
            string key = innerKvp.Key;
            string labelText = innerKvp.Value;

            float maxWidth = Screen.width - 20;
            Vector2 labelSize = labelStyle.CalcSize(new GUIContent(labelText));
            labelSize.x = Mathf.Min(labelSize.x, maxWidth);

            float xPosition = Screen.width - labelSize.x - 10;
            Rect labelRect = new(xPosition, yPosition, labelSize.x, labelSize.y);

            GUI.Label(labelRect, labelText, labelStyle);
            yPosition += labelSize.y + 5;
        }
    }
}
