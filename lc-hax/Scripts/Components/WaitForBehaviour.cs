using System;
using System.Collections;

internal class WaitForBehaviour : MonoBehaviour {
    Action? Action { get; set; }
    Func<float, bool>? TimerPredicate { get; set; }
    Func<bool>? Predicate { get; set; }

    internal void Init(Action action) {
        this.Action = action;
        _ = this.Predicate is null
            ? this.StartCoroutine(this.WaitForTimerPredicateCoroutine())
            : this.StartCoroutine(this.WaitForPredicateCoroutine());
    }

    internal WaitForBehaviour SetPredicate(Func<float, bool> predicate) {
        this.TimerPredicate = predicate;
        return this;
    }

    internal WaitForBehaviour SetPredicate(Func<bool> predicate) {
        this.Predicate = predicate;
        return this;
    }

    IEnumerator WaitForPredicateCoroutine() {
        yield return new WaitUntil(this.Predicate);
        this.Finalise();
    }

    IEnumerator WaitForTimerPredicateCoroutine() {
        WaitForEndOfFrame waitForEndOfFrame = new();
        float timer = 0.0f;

        while (true) {
            if (this.TimerPredicate?.Invoke(timer) is not false) break;
            timer += Time.deltaTime;
            yield return waitForEndOfFrame;
        }

        this.Finalise();
    }

    void Finalise() {
        this.Action?.Invoke();
        Destroy(this.gameObject);
    }
}
