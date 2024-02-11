internal readonly struct RendererPair<T, R>(T gameObject, R renderer) where T : Object where R : Renderer {
    internal readonly T GameObject { get; } = gameObject;
    internal readonly R Renderer { get; } = renderer;
}
