using System;
using System.Collections;
using UnityEngine;

namespace Hax;

public class WaitFor : MonoBehaviour {
    Action? Action { get; set; }
    Func<bool>? Predicate { get; set; }

    public void Init(Func<bool> predicate, Action action) {
        this.Predicate = predicate;
        this.Action = action;
        _ = this.StartCoroutine(this.WaitForCoroutine());
    }

    IEnumerator WaitForCoroutine() {
        while (true) {
            if (this.Predicate is not null && this.Predicate()) break;
            yield return new WaitForEndOfFrame();
        }

        this.Action?.Invoke();
        Destroy(this.gameObject);
    }
}
