using Hax;
using UnityEngine;

internal sealed class CrosshairMod : MonoBehaviour
{
    private const float GapSize = 7.0f;
    private const float Thickness = 3.0f;
    private const float Length = 10.0f;

    private Vector2 TopCrosshairPosition { get; set; }
    private Vector2 BottomCrosshairPosition { get; set; }
    private Vector2 LeftCrosshairPosition { get; set; }
    private Vector2 RightCrosshairPosition { get; set; }

    private bool InGame { get; set; } = false;

    private void OnEnable()
    {
        ScreenListener.OnScreenSizeChange += InitialiseCrosshairPositions;
        GameListener.OnGameStart += ToggleInGame;
        GameListener.OnGameEnd += ToggleNotInGame;
    }

    private void OnDisable()
    {
        ScreenListener.OnScreenSizeChange -= InitialiseCrosshairPositions;
        GameListener.OnGameStart -= ToggleInGame;
        GameListener.OnGameEnd -= ToggleNotInGame;
    }

    private void Start()
    {
        InitialiseCrosshairPositions();
    }

    private void OnGUI()
    {
        if (!InGame) return;
        RenderCrosshair();
    }

    private void ToggleInGame()
    {
        InGame = true;
    }

    private void ToggleNotInGame()
    {
        InGame = false;
    }

    private void InitialiseCrosshairPositions()
    {
        var screenCentre = Helper.GetScreenCentre();
        var halfWidth = 0.5f * Thickness;
        var lengthToCentre = GapSize + Length;
        var topLeftX = screenCentre.x - halfWidth;
        var topLeftY = screenCentre.y - halfWidth;

        TopCrosshairPosition = new Vector2(topLeftX, screenCentre.y - lengthToCentre);
        BottomCrosshairPosition = new Vector2(topLeftX, screenCentre.y + GapSize);
        RightCrosshairPosition = new Vector2(screenCentre.x + GapSize, topLeftY);
        LeftCrosshairPosition = new Vector2(screenCentre.x - lengthToCentre, topLeftY);
    }

    private void RenderCrosshair()
    {
        Size verticalSize = new()
        {
            Width = Thickness,
            Height = Length
        };

        Size horizontalSize = new()
        {
            Width = Length,
            Height = Thickness
        };

        Helper.DrawBox(TopCrosshairPosition, verticalSize);
        Helper.DrawBox(BottomCrosshairPosition, verticalSize);
        Helper.DrawBox(RightCrosshairPosition, horizontalSize);
        Helper.DrawBox(LeftCrosshairPosition, horizontalSize);
    }
}