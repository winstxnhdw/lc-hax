using UnityEngine;

sealed class CachedLabel {
    internal GUIContent Content { get; } = new(string.Empty);

    internal string Text {
        get => this.Content.text;
        set {
            if (this.Content.text == value) return;
            this.Content.text = value;
            this.HasMeasuredSize = false;
        }
    }

    bool HasMeasuredSize { get; set; }
    int StyleGeneration { get; set; }
    Vector2 Size { get; set; }

    internal Vector2 GetSize(GUIStyle style, int styleGeneration) {
        if (this.HasMeasuredSize && this.StyleGeneration == styleGeneration) return this.Size;

        this.Size = style.CalcSize(this.Content);
        this.StyleGeneration = styleGeneration;
        this.HasMeasuredSize = true;
        return this.Size;
    }
}
