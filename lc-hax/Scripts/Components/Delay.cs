using System;
using UnityEngine;

namespace Hax;

public class DelayObject : MonoBehaviour {
    Action? Action { get; set; }
    float ExpireTime { get; set; } = 0.0f;
    float Timer { get; set; } = 0.0f;

    public DelayObject Init(Action action, float expireTime) {
        this.Action = action;
        this.ExpireTime = expireTime;

        return this;
    }

    void Update() {
        if (this.Timer > this.ExpireTime) {
            this.Action?.Invoke();
            Destroy(this.gameObject);
            return;
        }

        float deltaTime = Time.deltaTime;
        this.Timer += deltaTime;
    }
}
