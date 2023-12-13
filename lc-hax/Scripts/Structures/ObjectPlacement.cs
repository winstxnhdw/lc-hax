using UnityEngine;

public readonly struct ObjectPlacement<T, M>(
    T targetObject,
    M gameObject,
    Vector3 positionOffset = new(),
    Vector3 rotationOffset = new()
) where T : Transform where M : MonoBehaviour {
    public readonly T TargetObject { get; } = targetObject;
    public readonly M GameObject { get; } = gameObject;
    public readonly Vector3 PositionOffset { get; } = positionOffset;
    public readonly Vector3 RotationOffset { get; } = rotationOffset;
}

public readonly struct ObjectPlacements<T, M>(
    ObjectPlacement<T, M> placement,
    ObjectPlacement<T, M> previousPlacement
) where T : Transform where M : MonoBehaviour {
    public readonly ObjectPlacement<T, M> Placement { get; } = placement;
    public readonly ObjectPlacement<T, M> PreviousPlacement { get; } = previousPlacement;
}
