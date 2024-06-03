#region

using System;
using System.Collections;
using UnityEngine;

#endregion

class TransientBehaviour : MonoBehaviour {
    Action<float>? Action { get; set; }
    Action? DisposeAction { get; set; }
    float ExpireTime { get; set; } = 0.0f;

    internal TransientBehaviour Init(Action<float> action, float expireTime, float delay = 0.0f) {
        this.Action = action;
        this.ExpireTime = expireTime;

        _ = delay > 0.0f
            ? this.StartCoroutine(this.TransientCoroutine(delay))
            : this.StartCoroutine(this.TransientCoroutine());

        return this;
    }

    internal void Dispose(Action disposeAction) => this.DisposeAction = disposeAction;

    internal void Unless(Func<bool> predicate) => this.StartCoroutine(this.UnlessCoroutine(predicate));

    IEnumerator UnlessCoroutine(Func<bool> predicate) {
        yield return new WaitUntil(predicate);
        this.Finalise();
    }

    IEnumerator TransientCoroutine(float delay) {
        WaitForSeconds waitForDelay = new(delay);
        float timeElapsed = 0.0f;

        while (this.ExpireTime > 0.0f) {
            this.ExpireTime -= delay;
            this.Action?.Invoke(timeElapsed += delay);

            yield return waitForDelay;
        }

        this.Finalise();
    }

    IEnumerator TransientCoroutine() {
        WaitForEndOfFrame waitForEndOfFrame = new();
        float timeElapsed = 0.0f;

        while (this.ExpireTime > 0.0f) {
            float deltaTime = Time.deltaTime;
            this.ExpireTime -= deltaTime;
            this.Action?.Invoke(timeElapsed += deltaTime);

            yield return waitForEndOfFrame;
        }

        this.Finalise();
    }

    void Finalise() {
        this.DisposeAction?.Invoke();
        Destroy(this.gameObject);
    }
}
