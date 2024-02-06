using System;
using UnityEngine;
using Hax;

internal class GameListener : MonoBehaviour {
    internal static event Action? onGameStart;
    internal static event Action? onGameEnd;
    internal static event Action? onShipLand;
    internal static event Action? onShipLeave;

    bool InGame { get; set; } = false;
    bool ShipLand { get; set; } = false;

    void Update() {
        this.InGameListener();
        this.ShipDoorListener();
    }

    void ShipDoorListener() {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;

        if (startOfRound.shipHasLanded && !this.ShipLand) {
            this.ShipLand = true;
            GameListener.onShipLand?.Invoke();
        }

        else if (!startOfRound.shipHasLanded && this.ShipLand) {
            this.ShipLand = false;
            GameListener.onShipLeave?.Invoke();
        }
    }

    void InGameListener() {
        bool inGame = Helper.LocalPlayer is not null;

        if (this.InGame == inGame) {
            return;
        }

        this.InGame = inGame;

        if (this.InGame) {
            GameListener.onGameStart?.Invoke();
        }

        else {
            GameListener.onGameEnd?.Invoke();
        }
    }
}
