using UnityEngine;
using Hax;

internal class CrosshairMod : MonoBehaviour {
    const float GapSize = 7.0f;
    const float Thickness = 3.0f;
    const float Length = 10.0f;

    Vector2 TopCrosshairPosition { get; set; }
    Vector2 BottomCrosshairPosition { get; set; }
    Vector2 LeftCrosshairPosition { get; set; }
    Vector2 RightCrosshairPosition { get; set; }

    bool InGame { get; set; } = false;

    void OnEnable() {
        ScreenListener.OnScreenSizeChange += this.InitialiseCrosshairPositions;
        GameListener.OnGameStart += this.ToggleInGame;
        GameListener.OnGameEnd += this.ToggleNotInGame;
    }

    void OnDisable() {
        ScreenListener.OnScreenSizeChange -= this.InitialiseCrosshairPositions;
        GameListener.OnGameStart -= this.ToggleInGame;
        GameListener.OnGameEnd -= this.ToggleNotInGame;
    }

    void Start() => this.InitialiseCrosshairPositions();

    void OnGUI() {
        if (!this.InGame) return;
        this.RenderCrosshair();
    }

    void ToggleInGame() => this.InGame = true;

    void ToggleNotInGame() => this.InGame = false;

    void InitialiseCrosshairPositions() {
        Vector2 screenCentre = Helper.GetScreenCentre();
        float halfWidth = 0.5f * CrosshairMod.Thickness;
        float lengthToCentre = CrosshairMod.GapSize + CrosshairMod.Length;
        float topLeftX = screenCentre.x - halfWidth;
        float topLeftY = screenCentre.y - halfWidth;

        this.TopCrosshairPosition = new Vector2(topLeftX, screenCentre.y - lengthToCentre);
        this.BottomCrosshairPosition = new Vector2(topLeftX, screenCentre.y + CrosshairMod.GapSize);
        this.RightCrosshairPosition = new Vector2(screenCentre.x + CrosshairMod.GapSize, topLeftY);
        this.LeftCrosshairPosition = new Vector2(screenCentre.x - lengthToCentre, topLeftY);
    }

    void RenderCrosshair() {
        Size verticalSize = new() {
            Width = CrosshairMod.Thickness,
            Height = CrosshairMod.Length
        };

        Size horizontalSize = new() {
            Width = CrosshairMod.Length,
            Height = CrosshairMod.Thickness
        };

        Helper.DrawBox(this.TopCrosshairPosition, verticalSize);
        Helper.DrawBox(this.BottomCrosshairPosition, verticalSize);
        Helper.DrawBox(this.RightCrosshairPosition, horizontalSize);
        Helper.DrawBox(this.LeftCrosshairPosition, horizontalSize);
    }
}
