internal readonly struct ObjectPlacement<T, M>(
    T targetObject,
    M gameObject,
    Vector3 positionOffset = new(),
    Vector3 rotationOffset = new()
) where T : Transform where M : MonoBehaviour {
    internal readonly T TargetObject { get; } = targetObject;
    internal readonly M GameObject { get; } = gameObject;
    internal readonly Vector3 PositionOffset { get; } = positionOffset;
    internal readonly Vector3 RotationOffset { get; } = rotationOffset;
}

internal readonly struct ObjectPlacements<T, M>(
    ObjectPlacement<T, M> placement,
    ObjectPlacement<T, M> previousPlacement
) where T : Transform where M : MonoBehaviour {
    internal readonly ObjectPlacement<T, M> Placement { get; } = placement;
    internal readonly ObjectPlacement<T, M> PreviousPlacement { get; } = previousPlacement;
}
