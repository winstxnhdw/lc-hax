using System;
using System.Collections;

internal class AsyncBehaviour : MonoBehaviour {
    Func<IEnumerator>? Func { get; set; }

    internal void Init(Func<IEnumerator> func) {
        this.Func = func;
        _ = this.StartCoroutine(this.AsyncCoroutine());
    }

    IEnumerator AsyncCoroutine() {
        yield return this.Func?.Invoke();
        Destroy(this.gameObject);
    }
}
