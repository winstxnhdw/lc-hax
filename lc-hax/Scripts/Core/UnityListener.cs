using System;
using UnityEngine;

public class UnityListener : MonoBehaviour
{
    public Action AwakeAction;
    public Action FixedUpdateAction;
    public Action LateUpdateAction;
    public Action OnDestroyAction;
    public Action OnDisableAction;
    public Action OnEnableAction;
    public Action StartAction;
    public Action UpdateAction;

    private void Awake()
    {
        AwakeAction?.Invoke();
    }

    private void Start()
    {
        StartAction?.Invoke();
    }

    private void Update()
    {
        UpdateAction?.Invoke();
    }

    private void FixedUpdate()
    {
        FixedUpdateAction?.Invoke();
    }

    private void LateUpdate()
    {
        LateUpdateAction?.Invoke();
    }

    private void OnDestroy()
    {
        OnDestroyAction?.Invoke();
    }

    private void OnDisable()
    {
        OnDisableAction?.Invoke();
    }

    private void OnEnable()
    {
        OnEnableAction?.Invoke();
    }
}