using System.Collections.Generic;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static List<RaycastHit> RaycastForward =>
        !Helper.CurrentCamera.IsNotNull(out Camera camera) || !camera.enabled
            ? []
            : [.. Physics.SphereCastAll(
                camera.transform.position + (camera.transform.forward * 1.5f),
                1.0f,
                camera.transform.forward,
                float.MaxValue
            )];
}
