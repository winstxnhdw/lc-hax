// using UnityEngine;

// namespace Hax;
// public class FakeCrosshair : MonoBehaviour {
//     float GapSize { get; } = 7.0f;
//     float Thickness { get; } = 3.0f;
//     float Length { get; } = 15.0f;

//     Vector2 TopCrosshairPosition { get; set; }
//     Vector2 BottomCrosshairPosition { get; set; }
//     Vector2 LeftCrosshairPosition { get; set; }
//     Vector2 RightCrosshairPosition { get; set; }

//     void Awake() {
//         this.InitialiseCrosshairPositions();
//     }

//     void OnGUI() {
//         this.RenderFakeCrosshair();
//     }

//     void InitialiseCrosshairPositions() {
//         float halfWidth = 0.5f * this.Thickness;
//         float lengthToCentre = this.GapSize + this.Length;
//         Vector2 screenCentre = ScreenInfo.GetScreenCentre();

//         this.TopCrosshairPosition = new Vector2(screenCentre.x - halfWidth, screenCentre.y - lengthToCentre);
//         this.RightCrosshairPosition = new Vector2(screenCentre.x + this.GapSize, screenCentre.y - halfWidth);
//         this.BottomCrosshairPosition = new Vector2(screenCentre.x - halfWidth, screenCentre.y + this.GapSize);
//         this.LeftCrosshairPosition = new Vector2(screenCentre.x - lengthToCentre, screenCentre.y - halfWidth);
//     }

//     void RenderFakeCrosshair() {
//         if (!MenuOptions.UseFakeCrosshair) return;

//         // Top crosshair
//         GUIHelper.DrawBox(this.TopCrosshairPosition, new Size(this.Thickness, this.Length));

//         // Right crosshair
//         GUIHelper.DrawBox(this.RightCrosshairPosition, new Size(this.Length, this.Thickness));

//         // Bottom crosshair
//         GUIHelper.DrawBox(this.BottomCrosshairPosition, new Size(this.Thickness, this.Length));

//         // Left crosshair
//         GUIHelper.DrawBox(this.LeftCrosshairPosition, new Size(this.Length, this.Thickness));
//     }
// }
