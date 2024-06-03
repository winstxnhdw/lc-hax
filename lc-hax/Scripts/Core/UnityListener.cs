#region

using System;
using UnityEngine;

#endregion

public class UnityListener : MonoBehaviour {
    public Action AwakeAction;
    public Action FixedUpdateAction;
    public Action LateUpdateAction;
    public Action OnDestroyAction;
    public Action OnDisableAction;
    public Action OnEnableAction;
    public Action StartAction;
    public Action UpdateAction;

    void Awake() => this.AwakeAction?.Invoke();

    void Start() => this.StartAction?.Invoke();

    void Update() => this.UpdateAction?.Invoke();

    void FixedUpdate() => this.FixedUpdateAction?.Invoke();

    void LateUpdate() => this.LateUpdateAction?.Invoke();

    void OnDestroy() => this.OnDestroyAction?.Invoke();

    void OnDisable() => this.OnDisableAction?.Invoke();

    void OnEnable() => this.OnEnableAction?.Invoke();
}
