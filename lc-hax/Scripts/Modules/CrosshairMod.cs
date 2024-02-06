using UnityEngine;
using Hax;

public class CrosshairMod : MonoBehaviour {
    const float gapSize = 7.0f;
    const float thickness = 3.0f;
    const float length = 10.0f;

    Vector2 TopCrosshairPosition { get; set; }
    Vector2 BottomCrosshairPosition { get; set; }
    Vector2 LeftCrosshairPosition { get; set; }
    Vector2 RightCrosshairPosition { get; set; }

    bool InGame { get; set; } = false;

    void OnEnable() {
        ScreenListener.onScreenSizeChange += this.InitialiseCrosshairPositions;
        GameListener.onGameStart += this.ToggleInGame;
        GameListener.onGameEnd += this.ToggleNotInGame;
    }

    void OnDisable() {
        ScreenListener.onScreenSizeChange -= this.InitialiseCrosshairPositions;
        GameListener.onGameStart -= this.ToggleInGame;
        GameListener.onGameEnd -= this.ToggleNotInGame;
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
        float halfWidth = 0.5f * CrosshairMod.thickness;
        float lengthToCentre = CrosshairMod.gapSize + CrosshairMod.length;
        float topLeftX = screenCentre.x - halfWidth;
        float topLeftY = screenCentre.y - halfWidth;

        this.TopCrosshairPosition = new Vector2(topLeftX, screenCentre.y - lengthToCentre);
        this.BottomCrosshairPosition = new Vector2(topLeftX, screenCentre.y + CrosshairMod.gapSize);
        this.RightCrosshairPosition = new Vector2(screenCentre.x + CrosshairMod.gapSize, topLeftY);
        this.LeftCrosshairPosition = new Vector2(screenCentre.x - lengthToCentre, topLeftY);
    }

    void RenderCrosshair() {
        Size verticalSize = new(CrosshairMod.thickness, CrosshairMod.length);
        Size horizontalSize = new(CrosshairMod.length, CrosshairMod.thickness);

        Helper.DrawBox(this.TopCrosshairPosition, verticalSize);
        Helper.DrawBox(this.BottomCrosshairPosition, verticalSize);
        Helper.DrawBox(this.RightCrosshairPosition, horizontalSize);
        Helper.DrawBox(this.LeftCrosshairPosition, horizontalSize);
    }
}
