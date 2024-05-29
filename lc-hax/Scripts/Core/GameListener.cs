using System;
using Hax;
using UnityEngine;

internal class GameListener : MonoBehaviour
{
    private bool InGame { get; set; } = false;
    private bool HasShipBegunDescent { get; set; } = false;
    private bool HasShipLeft { get; set; } = false;
    internal static event Action? OnGameStart;
    internal static event Action? OnGameEnd;
    internal static event Action? OnShipDescent;
    internal static event Action? OnShipLeave;
    internal static event Action? OnLevelGenerated;

    private void OnEnable()
    {
        LevelDependencyPatch.OnFinishLevelGeneration += OnFinishLevelGeneration;
    }

    private void OnDisable()
    {
        LevelDependencyPatch.OnFinishLevelGeneration -= OnFinishLevelGeneration;
    }

    private void LateUpdate()
    {
        InGameListener();
        ShipListener();
    }

    private void OnFinishLevelGeneration()
    {
        OnLevelGenerated?.Invoke();
    }

    private void ShipListener()
    {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (!startOfRound.inShipPhase && !HasShipBegunDescent)
        {
            HasShipBegunDescent = true;
            HasShipLeft = false;
            OnShipDescent?.Invoke();
        }

        else if (startOfRound.shipIsLeaving && !HasShipLeft)
        {
            HasShipLeft = true;
            HasShipBegunDescent = false;
            OnShipLeave?.Invoke();
        }
    }

    private void InGameListener()
    {
        var inGame = Helper.LocalPlayer is not null;

        if (InGame == inGame) return;

        InGame = inGame;

        if (InGame)
            OnGameStart?.Invoke();

        else
            OnGameEnd?.Invoke();
    }
}