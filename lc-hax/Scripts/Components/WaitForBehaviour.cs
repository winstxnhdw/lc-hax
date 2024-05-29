using System;
using System.Collections;
using UnityEngine;

internal class WaitForBehaviour : MonoBehaviour
{
    private Action? Action { get; set; }
    private Func<float, bool>? TimerPredicate { get; set; }
    private Func<bool>? Predicate { get; set; }

    internal void Init(Action action)
    {
        Action = action;
        _ = Predicate is null
            ? StartCoroutine(WaitForTimerPredicateCoroutine())
            : StartCoroutine(WaitForPredicateCoroutine());
    }

    internal WaitForBehaviour SetPredicate(Func<float, bool> predicate)
    {
        TimerPredicate = predicate;
        return this;
    }

    internal WaitForBehaviour SetPredicate(Func<bool> predicate)
    {
        Predicate = predicate;
        return this;
    }

    private IEnumerator WaitForPredicateCoroutine()
    {
        yield return new WaitUntil(Predicate);
        Finalise();
    }

    private IEnumerator WaitForTimerPredicateCoroutine()
    {
        WaitForEndOfFrame waitForEndOfFrame = new();
        var timer = 0.0f;

        while (true)
        {
            if (TimerPredicate?.Invoke(timer) is not false) break;
            timer += Time.deltaTime;
            yield return waitForEndOfFrame;
        }

        Finalise();
    }

    private void Finalise()
    {
        Action?.Invoke();
        Destroy(gameObject);
    }
}