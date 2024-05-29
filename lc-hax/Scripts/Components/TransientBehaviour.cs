using System;
using System.Collections;
using UnityEngine;

internal class TransientBehaviour : MonoBehaviour
{
    private Action<float>? Action { get; set; }
    private Action? DisposeAction { get; set; }
    private float ExpireTime { get; set; } = 0.0f;

    internal TransientBehaviour Init(Action<float> action, float expireTime, float delay = 0.0f)
    {
        Action = action;
        ExpireTime = expireTime;

        _ = delay > 0.0f
            ? StartCoroutine(TransientCoroutine(delay))
            : StartCoroutine(TransientCoroutine());

        return this;
    }

    internal void Dispose(Action disposeAction)
    {
        DisposeAction = disposeAction;
    }

    internal void Unless(Func<bool> predicate)
    {
        StartCoroutine(UnlessCoroutine(predicate));
    }

    private IEnumerator UnlessCoroutine(Func<bool> predicate)
    {
        yield return new WaitUntil(predicate);
        Finalise();
    }

    private IEnumerator TransientCoroutine(float delay)
    {
        WaitForSeconds waitForDelay = new(delay);
        var timeElapsed = 0.0f;

        while (ExpireTime > 0.0f)
        {
            ExpireTime -= delay;
            Action?.Invoke(timeElapsed += delay);

            yield return waitForDelay;
        }

        Finalise();
    }

    private IEnumerator TransientCoroutine()
    {
        WaitForEndOfFrame waitForEndOfFrame = new();
        var timeElapsed = 0.0f;

        while (ExpireTime > 0.0f)
        {
            var deltaTime = Time.deltaTime;
            ExpireTime -= deltaTime;
            Action?.Invoke(timeElapsed += deltaTime);

            yield return waitForEndOfFrame;
        }

        Finalise();
    }

    private void Finalise()
    {
        DisposeAction?.Invoke();
        Destroy(gameObject);
    }
}