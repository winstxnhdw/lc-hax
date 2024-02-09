using System;
using UnityEngine;
using Hax;

internal class GameListener : MonoBehaviour {
    internal static event Action? OnGameStart;
    internal static event Action? OnGameEnd;
    internal static event Action? OnShipDescent;
    internal static event Action? OnShipLeave;
    internal static event Action? OnLevelGenerated;

    bool InGame { get; set; } = false;
    bool HasShipBegunDescent { get; set; } = false;
    bool HasShipLeft { get; set; } = false;

    void OnEnable() => LevelDependencyPatch.OnFinishLevelGeneration += GameListener.OnLevelGenerated;

    void OnDisable() => LevelDependencyPatch.OnFinishLevelGeneration -= GameListener.OnLevelGenerated;

    void Update() {
        this.InGameListener();
        this.ShipListener();
    }

    void ShipListener() {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (!startOfRound.inShipPhase && !this.HasShipBegunDescent) {
            this.HasShipBegunDescent = true;
            this.HasShipLeft = false;
            GameListener.OnShipDescent?.Invoke();
        }

        else if (startOfRound.shipIsLeaving && !this.HasShipLeft) {
            this.HasShipLeft = true;
            this.HasShipBegunDescent = false;
            GameListener.OnShipLeave?.Invoke();
        }
    }

    void InGameListener() {
        bool inGame = Helper.LocalPlayer is not null;

        if (this.InGame == inGame) {
            return;
        }

        this.InGame = inGame;

        if (this.InGame) {
            GameListener.OnGameStart?.Invoke();
        }

        else {
            GameListener.OnGameEnd?.Invoke();
        }
    }
}
