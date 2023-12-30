using System;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static RaycastHit[] RaycastForward(Transform transform, float sphereRadius = 1.0f) {
        try {
            return Physics.SphereCastAll(
                transform.position + (transform.forward * (sphereRadius + 1.75f)),
                sphereRadius,
                transform.forward,
                float.MaxValue
            );
        }

        catch (NullReferenceException) {
            return [];
        }
    }
}
