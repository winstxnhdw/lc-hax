using System;
using UnityEngine;

namespace Hax;

public class TransientObject : MonoBehaviour {
    Action<float>? Action { get; set; }
    Action? DisposeAction { get; set; }
    float ExpireTime { get; set; } = 0.0f;
    float Timer { get; set; } = 0.0f;

    public TransientObject Init(Action<float> action, float expireTime) {
        this.Action = action;
        this.ExpireTime = expireTime;

        return this;
    }

    public TransientObject Dispose(Action disposeAction) {
        this.DisposeAction = disposeAction;
        return this;
    }

    void Update() {
        if (this.Timer >= this.ExpireTime) {
            this.DisposeAction?.Invoke();
            Destroy(this.gameObject);
            return;
        }

        float deltaTime = Time.deltaTime;
        this.Timer += deltaTime;
        this.Action?.Invoke(deltaTime);
    }
}
