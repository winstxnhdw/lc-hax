using System;
using UnityEngine;

namespace Hax;

public class GameListener : MonoBehaviour {
    public static event Action? onGameStart;
    public static event Action? onGameEnd;

    bool InGame { get; set; } = false;

    void Update() {
        this.InGameListener();
    }

    void InGameListener() {
        bool inGame = Helper.LocalPlayer != null;

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
