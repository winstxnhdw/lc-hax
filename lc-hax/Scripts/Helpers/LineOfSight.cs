using UnityEngine;

static partial class Helper {
    internal static bool HasLineOfSightTo(this Transform origin, Vector3 position, int layerMask) =>
        !Physics.Linecast(origin.position, position, layerMask, QueryTriggerInteraction.Ignore) &&
        !Physics.Linecast(position, origin.position, layerMask, QueryTriggerInteraction.Ignore);
}
