using System;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static int SphereCastForward(this RaycastHit[] array, Transform transform, float sphereRadius = 1.0f) {
        try {
            return Physics.SphereCastNonAlloc(
                transform.position + (transform.forward * (sphereRadius + 1.75f)),
                sphereRadius,
                transform.forward,
                array,
                float.MaxValue
            );
        }

        catch (NullReferenceException) {
            return 0;
        }
    }

    public static RaycastHit[] SphereCastForward(this Transform transform, float sphereRadius = 1.0f) {
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
