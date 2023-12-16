using System.Collections.Generic;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static List<RaycastHit> RaycastForward(float sphereRadius = 1.0f) {
        return !Helper.CurrentCamera.IsNotNull(out Camera camera) || !camera.enabled
            ? []
            : [.. Physics.SphereCastAll(
                camera.transform.position + (camera.transform.forward * (sphereRadius + 1.75f)),
                sphereRadius,
                camera.transform.forward,
                float.MaxValue
            )];
    }
}
