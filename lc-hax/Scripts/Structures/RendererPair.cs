using UnityEngine;

public readonly struct RendererPair<T, R>(T gameObject, R renderer) where T : Object where R : Renderer {
    public readonly T GameObject { get; } = gameObject;
    public readonly R Renderer { get; } = renderer;
}
