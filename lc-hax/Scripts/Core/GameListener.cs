#region

using System;
using Hax;
using UnityEngine;

#endregion

class GameListener : MonoBehaviour {
    bool InGame { get; set; } = false;
    bool HasShipBegunDescent { get; set; } = false;
    bool HasShipLeft { get; set; } = false;
    internal static event Action? OnGameStart;
    internal static event Action? OnGameEnd;
    internal static event Action? OnShipDescent;
    internal static event Action? OnShipLeave;
    internal static event Action? OnLevelGenerated;

    void OnEnable() => LevelDependencyPatch.OnFinishLevelGeneration += this.OnFinishLevelGeneration;

    void OnDisable() => LevelDependencyPatch.OnFinishLevelGeneration -= this.OnFinishLevelGeneration;

    void LateUpdate() {
        this.InGameListener();
        this.ShipListener();
    }

    void OnFinishLevelGeneration() => OnLevelGenerated?.Invoke();

    void ShipListener() {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (!startOfRound.inShipPhase && !this.HasShipBegunDescent) {
            this.HasShipBegunDescent = true;
            this.HasShipLeft = false;
            OnShipDescent?.Invoke();
        }

        else if (startOfRound.shipIsLeaving && !this.HasShipLeft) {
            this.HasShipLeft = true;
            this.HasShipBegunDescent = false;
            OnShipLeave?.Invoke();
        }
    }

    void InGameListener() {
        bool inGame = Helper.LocalPlayer is not null;

        if (this.InGame == inGame) return;

        this.InGame = inGame;

        if (this.InGame)
            OnGameStart?.Invoke();

        else
            OnGameEnd?.Invoke();
    }
}
