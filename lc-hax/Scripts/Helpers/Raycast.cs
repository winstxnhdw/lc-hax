using System;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static RaycastHit[] RaycastForward(float sphereRadius = 1.0f) {
        if (!Helper.CurrentCamera.IsNotNull(out Camera camera)) return [];

        try {
            return Physics.SphereCastAll(
                camera.transform.position + (camera.transform.forward * (sphereRadius + 1.75f)),
                sphereRadius,
                camera.transform.forward,
                float.MaxValue
            );
        }

        catch (NullReferenceException) {
            return [];
        }
    }
}
