using UnityEngine;

static partial class Helper {
    static GUIStyle? CachedLabelStyle { get; set; }
    static GUIContent LabelContent { get; } = new();

    internal static void DrawLabel(Vector2 position, string label, Color colour) {
        GUIStyle labelStyle = Helper.CachedLabelStyle ??= new GUIStyle(GUI.skin.label) {
            fontStyle = FontStyle.Bold
        };

        Helper.LabelContent.text = label;
        Vector2 size = labelStyle.CalcSize(Helper.LabelContent);
        Vector2 newPosition = position - (size * 0.5f);
        Rect labelBounds = new(newPosition.x, newPosition.y, size.x + 1, size.y + 1);

        if (!labelBounds.Overlaps(new Rect(0, 0, Screen.width, Screen.height))) {
            return;
        }

        labelStyle.normal.textColor = Color.black;
        GUI.Label(new Rect(newPosition.x, newPosition.y, size.x, size.y), Helper.LabelContent, labelStyle);

        labelStyle.normal.textColor = colour;
        GUI.Label(new Rect(newPosition.x + 1, newPosition.y + 1, size.x, size.y), Helper.LabelContent, labelStyle);
    }

    internal static void DrawLabel(Vector2 position, string label) => Helper.DrawLabel(position, label, Color.white);

    internal static void DrawOutlineBox(Vector2 centrePosition, Size size, float lineWidth, Color colour) {
        float halfWidth = 0.5f * size.Width;
        float halfHeight = 0.5f * size.Height;

        float leftX = centrePosition.x - halfWidth;
        float rightX = centrePosition.x + halfWidth;
        float topY = centrePosition.y - halfHeight;
        float bottomY = centrePosition.y + halfHeight;

        Size horizontalSize = size with { Height = lineWidth };
        Size verticalSize = size with { Width = lineWidth };

        Vector2 topLeft = new(leftX, topY);
        Helper.DrawBox(topLeft, horizontalSize, colour);

        Vector2 rightBorderTopLeft = new(rightX - lineWidth, topY);
        Helper.DrawBox(rightBorderTopLeft, verticalSize, colour);

        Vector2 bottomBorderTopLeft = new(leftX, bottomY - lineWidth);
        Helper.DrawBox(bottomBorderTopLeft, horizontalSize, colour);

        Helper.DrawBox(topLeft, verticalSize, colour);
    }

    internal static void DrawOutlineBox(Vector2 centrePosition, Size size, float lineWidth) =>
        Helper.DrawOutlineBox(centrePosition, size, lineWidth, Color.white);

    internal static void DrawBox(Vector2 position, Size size, Color colour) {
        Color previousCOlour = GUI.color;
        GUI.color = colour;

        Rect rect = new(position.x, position.y, size.Width, size.Height);
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill);

        GUI.color = previousCOlour;
    }

    internal static void DrawBox(Vector2 position, Size size) => Helper.DrawBox(position, size, Color.white);
}
