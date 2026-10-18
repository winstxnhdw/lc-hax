using UnityEngine;

static partial class Helper {
    internal static DoorLock? GetDoor(this Collider collider) =>
        collider.GetComponentInParent<DoorLock>().Unfake() ??
        collider.GetComponent<TriggerPointToDoor>().Unfake()?.pointToDoor.Unfake();
}
