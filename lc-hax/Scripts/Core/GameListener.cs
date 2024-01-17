using System;
using UnityEngine;

namespace Hax;

public class GameListener : MonoBehaviour {
    public static event Action? onGameStart;
    public static event Action? onGameEnd;
    public static event Action? onShipLand;
    public static event Action? onShipLeave;

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
