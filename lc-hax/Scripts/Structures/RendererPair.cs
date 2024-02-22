using UnityEngine;

readonly record struct RendererPair<T, R> where T : Object where R : Renderer {
    internal required T GameObject { get; init; }
    internal required R Renderer { get; init; }
}
