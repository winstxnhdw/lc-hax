using System;
using System.Collections;
using UnityEngine;

namespace Hax;

public class WaitForBehaviour : MonoBehaviour {
    Action? Action { get; set; }
    Func<float, bool>? Predicate { get; set; }

    public void Init(Action action) {
        this.Action = action;
        _ = this.StartCoroutine(this.WaitForPredicateCoroutine());
    }

    public WaitForBehaviour SetPredicate(Func<float, bool> predicate) {
        this.Predicate = predicate;
        return this;
    }

    IEnumerator WaitForPredicateCoroutine() {
        float timer = 0.0f;

        while (true) {
            if (this.Predicate is not null && this.Predicate(timer)) break;
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        this.Action?.Invoke();
        Destroy(this.gameObject);
    }
}
