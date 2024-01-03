using UnityEngine;

public readonly struct RendererPair<T>(T gameObject, Renderer renderer) where T : Component {
    public readonly Renderer Renderer { get; } = renderer;
    public readonly T GameObject { get; } = gameObject;
}
