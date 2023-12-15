using System.Collections.Generic;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static List<RaycastHit> RaycastForward =>
        !Helper.CurrentCamera.IsNotNull(out Camera camera) || !camera.enabled
            ? []
            : [.. Physics.SphereCastAll(
                camera.transform.position + (camera.transform.forward * 0.5f),
                1f,
                camera.transform.forward,
                float.MaxValue
            )];
}
