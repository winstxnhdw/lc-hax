#region

using UnityEngine;

#endregion

namespace Hax;

static partial class Helper {
    internal static void DrawLabel(Vector2 position, string label, Color colour) {
        GUIStyle labelStyle = new(GUI.skin.label) {
            fontStyle = FontStyle.Bold
        };

        Vector2 size = labelStyle.CalcSize(new GUIContent(label));
        Vector2 newPosition = position - size * 0.5f;

        labelStyle.normal.textColor = Color.black;
        GUI.Label(new Rect(newPosition.x, newPosition.y, size.x, size.y), label, labelStyle);

        labelStyle.normal.textColor = colour;
        GUI.Label(new Rect(newPosition.x + 1, newPosition.y + 1, size.x, size.y), label, labelStyle);
    }

    internal static void DrawLabel(Vector2 position, string label) => DrawLabel(position, label, Color.white);

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
        DrawBox(topLeft, horizontalSize, colour);

        Vector2 rightBorderTopLeft = new(rightX - lineWidth, topY);
        DrawBox(rightBorderTopLeft, verticalSize, colour);

        Vector2 bottomBorderTopLeft = new(leftX, bottomY - lineWidth);
        DrawBox(bottomBorderTopLeft, horizontalSize, colour);

        DrawBox(topLeft, verticalSize, colour);
    }

    internal static void DrawOutlineBox(Vector2 centrePosition, Size size, float lineWidth) =>
        DrawOutlineBox(centrePosition, size, lineWidth, Color.white);

    internal static void DrawBox(Vector2 position, Size size, Color colour) {
        Color previousCOlour = GUI.color;
        GUI.color = colour;

        Rect rect = new(position.x, position.y, size.Width, size.Height);
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill);

        GUI.color = previousCOlour;
    }

    internal static void DrawBox(Vector2 position, Size size) => DrawBox(position, size, Color.white);
}
