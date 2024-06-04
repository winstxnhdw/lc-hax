#region

using System;
using UnityEngine;

#endregion

namespace Hax {
    static partial class Helper {
        /// <summary>
        /// Perform a sphere cast forward from the specified transform.
        /// </summary>
        /// <param name="array">Array to store the resulting RaycastHit objects.</param>
        /// <param name="transform">The transform from which to perform the sphere cast.</param>
        /// <param name="sphereRadius">The radius of the sphere to cast.</param>
        /// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
        /// <param name="collisionInteraction">Specifies how queries hit triggers.</param>
        /// <returns>The number of hits detected.</returns>
        internal static int SphereCastForward(this RaycastHit[] array, Transform transform, float sphereRadius = 1.0f, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction collisionInteraction = QueryTriggerInteraction.UseGlobal) {
            try {
                return Physics.SphereCastNonAlloc(
                    transform.position + transform.forward * (sphereRadius + 1.75f),
                    sphereRadius,
                    transform.forward,
                    array,
                    float.MaxValue,
                    layerMask,
                    collisionInteraction
                );
            }
            catch (NullReferenceException) {
                return 0;
            }
        }

        /// <summary>
        /// Perform a sphere cast forward from the specified transform, detecting both triggers and colliders.
        /// </summary>
        /// <param name="transform">The transform from which to perform the sphere cast.</param>
        /// <param name="sphereRadius">The radius of the sphere to cast.</param>
        /// <param name="layerMask">A Layer mask that is used to selectively ignore colliders when casting a ray.</param>
        /// <param name="collisionInteraction">Specifies how queries hit triggers.</param>
        /// <returns>An array of RaycastHit objects representing all hits.</returns>
        internal static RaycastHit[] SphereCastForward(this Transform transform, float sphereRadius = 1.0f, int layerMask = Physics.DefaultRaycastLayers, QueryTriggerInteraction collisionInteraction = QueryTriggerInteraction.UseGlobal) {
            try {
                return Physics.SphereCastAll(
                    transform.position + transform.forward * (sphereRadius + 1.75f),
                    sphereRadius,
                    transform.forward,
                    float.MaxValue,
                    layerMask,
                    collisionInteraction
                );
            }
            catch (NullReferenceException) {
                return Array.Empty<RaycastHit>();
            }
        }
    }
}
