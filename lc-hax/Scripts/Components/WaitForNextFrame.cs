using System;
using System.Collections;
using UnityEngine;

namespace Hax;

public class WaitForNextFrame : MonoBehaviour {
    Action? Action { get; set; }

    public void Init(Action action) {
        this.Action = action;
        _ = this.StartCoroutine(this.WaitForNextFrameCoroutine());
    }

    IEnumerator WaitForNextFrameCoroutine() {
        yield return new WaitForEndOfFrame();
        this.Action?.Invoke();
        Destroy(this.gameObject);
    }
}
