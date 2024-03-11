using System;
using System.Collections;
using UnityEngine;

class AsyncBehaviour : MonoBehaviour {
    Func<IEnumerator>? Func { get; set; }

    internal void Init(Func<IEnumerator> func) {
        this.Func = func;
        _ = this.StartCoroutine(AsyncCoroutine());
    }

    private IEnumerator AsyncCoroutine() {
        if (this.Func != null) {
            yield return this.StartCoroutine(this.Func());
        }
        Destroy(this.gameObject);
    }
}
