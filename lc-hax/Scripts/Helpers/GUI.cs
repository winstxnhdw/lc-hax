using UnityEngine;

readonly record struct LabelStyleSettings(
    Font? Font,
    Font? DefaultFont,
    int FontSize,
    bool RichText,
    ImagePosition ImagePosition,
    float FixedWidth,
    float FixedHeight,
    Vector4 Padding
);

static partial class Helper {
    static GUIStyle? CachedLabelStyle { get; set; }
    static GUIStyle? SourceLabelStyle { get; set; }
    static LabelStyleSettings CachedLabelStyleSettings { get; set; }
    static int LabelStyleGeneration { get; set; }
    static bool IsLabelFontListenerRegistered { get; set; }

    internal static GUIStyle BeginLabelRepaint() {
        if (!Helper.IsLabelFontListenerRegistered) {
            Font.textureRebuilt += Helper.OnLabelFontTextureRebuilt;
            Helper.IsLabelFontListenerRegistered = true;
        }

        GUISkin skin = GUI.skin;
        GUIStyle sourceStyle = skin.label;
        RectOffset padding = sourceStyle.padding;
        LabelStyleSettings settings = new(
            sourceStyle.font,
            skin.font,
            sourceStyle.fontSize,
            sourceStyle.richText,
            sourceStyle.imagePosition,
            sourceStyle.fixedWidth,
            sourceStyle.fixedHeight,
            new Vector4(padding.left, padding.right, padding.top, padding.bottom)
        );

        if (Helper.CachedLabelStyle is GUIStyle labelStyle &&
            object.ReferenceEquals(Helper.SourceLabelStyle, sourceStyle) &&
            Helper.CachedLabelStyleSettings == settings) {
            return labelStyle;
        }

        Helper.CachedLabelStyle = new GUIStyle(sourceStyle) {
            fontStyle = FontStyle.Bold
        };

        Helper.SourceLabelStyle = sourceStyle;
        Helper.CachedLabelStyleSettings = settings;
        Helper.LabelStyleGeneration++;

        return Helper.CachedLabelStyle;
    }

    static void OnLabelFontTextureRebuilt(Font _) => Helper.LabelStyleGeneration++;

    internal static void DrawLabel(Vector2 position, CachedLabel label, Color colour, Rect screenBounds) {
        GUIStyle labelStyle = Helper.CachedLabelStyle ?? Helper.BeginLabelRepaint();
        Vector2 size = label.GetSize(labelStyle, Helper.LabelStyleGeneration);
        Vector2 newPosition = position - (size * 0.5f);
        Rect labelBounds = new(newPosition.x, newPosition.y, size.x + 1, size.y + 1);

        if (!labelBounds.Overlaps(screenBounds)) {
            return;
        }

        labelStyle.normal.textColor = Color.black;
        GUI.Label(new Rect(newPosition.x, newPosition.y, size.x, size.y), label.Content, labelStyle);

        labelStyle.normal.textColor = colour;
        GUI.Label(new Rect(newPosition.x + 1, newPosition.y + 1, size.x, size.y), label.Content, labelStyle);
    }

    internal static void DrawLabel(Vector2 position, CachedLabel label, Rect screenBounds) =>
        Helper.DrawLabel(position, label, Color.white, screenBounds);

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
