using System;
using System.Collections;
using UnityEngine;

namespace Hax;

public class TransientBehaviour : MonoBehaviour {
    Action<float>? Action { get; set; }
    Action? DisposeAction { get; set; }
    float ExpireTime { get; set; } = 0.0f;
    float Delay { get; set; } = 0.0f;

    public TransientBehaviour Init(Action<float> action, float expireTime, float delay = 0.0f) {
        this.Action = action;
        this.ExpireTime = expireTime;
        this.Delay = delay;

        _ = this.Delay > 0.0f
            ? this.StartCoroutine(this.TransientCoroutineWithDelay())
            : this.StartCoroutine(this.TransientCoroutine());

        return this;
    }

    public void Dispose(Action disposeAction) {
        this.DisposeAction = disposeAction;
    }

    IEnumerator TransientCoroutineWithDelay() {
        while (this.ExpireTime > 0.0f) {
            float deltaTime = Time.deltaTime;
            this.ExpireTime -= deltaTime;
            this.Action?.Invoke(deltaTime);

            yield return new WaitForSeconds(this.Delay);
        }

        this.DisposeAction?.Invoke();
        Destroy(this.gameObject);
    }

    IEnumerator TransientCoroutine() {
        while (this.ExpireTime > 0.0f) {
            float deltaTime = Time.deltaTime;
            this.ExpireTime -= deltaTime;
            this.Action?.Invoke(deltaTime);

            yield return new WaitForEndOfFrame();
        }

        this.DisposeAction?.Invoke();
        Destroy(this.gameObject);
    }
}
