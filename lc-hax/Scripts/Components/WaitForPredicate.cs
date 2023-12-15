using System;
using System.Collections;
using UnityEngine;

namespace Hax;

public class WaitForPredicate : MonoBehaviour {
    Action? Action { get; set; }
    Func<bool>? Predicate { get; set; }

    public void Init(Action action) {
        this.Action = action;
        _ = this.StartCoroutine(this.WaitForPredicateCoroutine());
    }

    public WaitForPredicate SetPredicate(Func<bool> predicate) {
        this.Predicate = predicate;
        return this;
    }

    IEnumerator WaitForPredicateCoroutine() {
        while (true) {
            if (this.Predicate is not null && this.Predicate()) break;
            yield return new WaitForEndOfFrame();
        }

        this.Action?.Invoke();
        Destroy(this.gameObject);
    }
}
