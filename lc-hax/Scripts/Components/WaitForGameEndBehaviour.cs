using System;
using UnityEngine;

internal class WaitForGameEndBehaviour : MonoBehaviour
{
    private Action? ActionBefore { get; set; }
    private Action? ActionAfter { get; set; }
    private bool HasGameEnded { get; set; } = false;

    private void OnEnable()
    {
        GameListener.OnGameEnd += OnGameEnd;
    }

    private void OnDisable()
    {
        GameListener.OnGameEnd -= OnGameEnd;
    }

    private void OnGameEnd()
    {
        HasGameEnded = true;
    }

    internal void AddActionBefore(Action action)
    {
        ActionBefore = action;
    }

    internal void AddActionAfter(Action action)
    {
        ActionAfter = action;
    }

    private void Update()
    {
        if (HasGameEnded)
        {
            Finalise();
            return;
        }

        ActionBefore?.Invoke();
    }

    private void Finalise()
    {
        ActionAfter?.Invoke();
        Destroy(gameObject);
    }
}