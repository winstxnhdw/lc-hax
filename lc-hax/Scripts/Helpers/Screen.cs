using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static Vector2 GetScreenCentre() => new Vector2(Screen.width, Screen.height) * 0.5f;

    public static Vector3? GetWorldScreenCentre3D(float z = 0.0f) {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return null;

        Vector3 screenCentre = GetScreenCentre();
        screenCentre.z = z;
        return camera.ScreenToWorldPoint(screenCentre);
    }

    public static Vector2? GetWorldScreenCentre2D() => Helper.GetWorldScreenCentre3D();
}
