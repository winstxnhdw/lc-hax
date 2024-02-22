using UnityEngine;

readonly record struct RendererPair<T, R> where T : Object where R : Renderer {
    internal readonly required T GameObject { get; init; }
    internal readonly required R Renderer { get; init; }
}
