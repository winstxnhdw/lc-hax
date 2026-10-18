using UnityEngine;

[Mod]
sealed class CrosshairMod : MonoBehaviour {
    Vector2 TopCrosshairPosition { get; set; }
    Vector2 BottomCrosshairPosition { get; set; }
    Vector2 LeftCrosshairPosition { get; set; }
    Vector2 RightCrosshairPosition { get; set; }

    bool InGame { get; set; }

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
        float halfWidth = 0.5f * Setting.CrosshairThickness;
        float lengthToCentre = Setting.CrosshairGap + Setting.CrosshairLength;
        float topLeftX = screenCentre.x - halfWidth;
        float topLeftY = screenCentre.y - halfWidth;

        this.TopCrosshairPosition = new Vector2(topLeftX, screenCentre.y - lengthToCentre);
        this.BottomCrosshairPosition = new Vector2(topLeftX, screenCentre.y + Setting.CrosshairGap);
        this.RightCrosshairPosition = new Vector2(screenCentre.x + Setting.CrosshairGap, topLeftY);
        this.LeftCrosshairPosition = new Vector2(screenCentre.x - lengthToCentre, topLeftY);
    }

    void RenderCrosshair() {
        Size verticalSize = new() {
            Width = Setting.CrosshairThickness,
            Height = Setting.CrosshairLength
        };

        Size horizontalSize = new() {
            Width = Setting.CrosshairLength,
            Height = Setting.CrosshairThickness
        };

        Helper.DrawBox(this.TopCrosshairPosition, verticalSize);
        Helper.DrawBox(this.BottomCrosshairPosition, verticalSize);
        Helper.DrawBox(this.RightCrosshairPosition, horizontalSize);
        Helper.DrawBox(this.LeftCrosshairPosition, horizontalSize);
    }
}
