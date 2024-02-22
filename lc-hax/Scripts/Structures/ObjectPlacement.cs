using UnityEngine;

readonly record struct ObjectPlacement<T, M>() where T : Transform where M : MonoBehaviour {
    internal required T TargetObject { get; init; }
    internal required M GameObject { get; init; }
    internal Vector3 PositionOffset { get; init; } = new();
    internal Vector3 RotationOffset { get; init; } = new();
}

readonly record struct ObjectPlacements<T, M> where T : Transform where M : MonoBehaviour {
    internal required ObjectPlacement<T, M> Placement { get; init; }
    internal required ObjectPlacement<T, M> PreviousPlacement { get; init; }
}
