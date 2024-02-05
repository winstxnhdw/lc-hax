using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static void DrawLabel(Vector2 position, string label, Color colour) {
        GUIStyle labelStyle = new(GUI.skin.label) {
            fontStyle = FontStyle.Bold
        };

        Vector2 size = labelStyle.CalcSize(new GUIContent(label));
        Vector2 newPosition = position - (size * 0.5f);

        labelStyle.normal.textColor = Color.black;
        GUI.Label(new Rect(newPosition.x, newPosition.y, size.x, size.y), label, labelStyle);

        labelStyle.normal.textColor = colour;
        GUI.Label(new Rect(newPosition.x + 1, newPosition.y + 1, size.x, size.y), label, labelStyle);
    }

    public static void DrawLabel(Vector2 position, string label) => Helper.DrawLabel(position, label, Color.white);

    public static void DrawOutlineBox(Vector2 centrePosition, Size size, float lineWidth, Color colour) {
        float halfWidth = 0.5f * size.Width;
        float halfHeight = 0.5f * size.Height;

        float leftX = centrePosition.x - halfWidth;
        float rightX = centrePosition.x + halfWidth;
        float topY = centrePosition.y - halfHeight;
        float bottomY = centrePosition.y + halfHeight;

        Vector2 topLeft = new(leftX, topY);
        Helper.DrawBox(topLeft, new Size(size.Width, lineWidth), colour);

        Vector2 rightBorderTopLeft = new(rightX - lineWidth, topY);
        Helper.DrawBox(rightBorderTopLeft, new Size(lineWidth, size.Height), colour);

        Vector2 bottomBorderTopLeft = new(leftX, bottomY - lineWidth);
        Helper.DrawBox(bottomBorderTopLeft, new Size(size.Width, lineWidth), colour);

        Helper.DrawBox(topLeft, new Size(lineWidth, size.Height), colour);
    }

    public static void DrawOutlineBox(Vector2 centrePosition, Size size, float lineWidth) =>
        Helper.DrawOutlineBox(centrePosition, size, lineWidth, Color.white);

    public static void DrawBox(Vector2 position, Size size, Color colour) {
        Color previousCOlour = GUI.color;
        GUI.color = colour;

        Rect rect = new(position.x, position.y, size.Width, size.Height);
        GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill);

        GUI.color = previousCOlour;
    }

    public static void DrawBox(Vector2 position, Size size) => Helper.DrawBox(position, size, Color.white);
}
