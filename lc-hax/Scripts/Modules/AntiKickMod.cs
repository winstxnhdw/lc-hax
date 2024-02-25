using UnityEngine;
using Hax;
using System.Collections;

sealed class AntiKickMod : MonoBehaviour {
    void OnEnable() {
        InputListener.OnBackslashPress += this.ToggleAntiKick;
        GameListener.OnGameStart += this.OnGameStart;
        GameListener.OnGameEnd += this.OnGameEnd;
    }

    void OnDisable() {
        InputListener.OnBackslashPress -= this.ToggleAntiKick;
        GameListener.OnGameStart -= this.OnGameStart;
        GameListener.OnGameEnd -= this.OnGameEnd;
    }

    IEnumerator RejoinLobby() {
        if (State.ConnectedLobby is not ConnectedLobby connectedLobby) yield break;

        WaitForEndOfFrame waitForEndOfFrame = new();

        while (Helper.FindObject<MenuManager>() is null) {
            yield return waitForEndOfFrame;
        }

        while (Helper.GameNetworkManager?.currentLobby.HasValue is not false) {
            yield return waitForEndOfFrame;
        }

        Helper.GameNetworkManager?.JoinLobby(connectedLobby.Lobby, connectedLobby.SteamId);
    }

    IEnumerator PrintInvisibleWarning() {
        yield return new WaitForSeconds(0.5f);
        Chat.Print("You are invisible! Do /invis to disable!");
    }

    bool FindJoinMessageInHistory() =>
        Helper.HUDManager?.ChatMessageHistory.First(message =>
            message.Contains($"{Helper.LocalPlayer?.playerUsername} joined the ship.")
        ) is not null;

    void OnGameEnd() {
        if (!State.DisconnectedVoluntarily && Setting.EnableAntiKick) {
            _ = this.StartCoroutine(this.RejoinLobby());
        }
    }

    void OnGameStart() {
        if (!Setting.EnableAntiKick || !Setting.EnableInvisible) return;

        Chat.Clear();
        _ = this.StartCoroutine(this.PrintInvisibleWarning());
    }

    void ToggleAntiKick() {
        if (Helper.LocalPlayer is not null) {
            Chat.Print("You cannot toggle anti-kick while in-game!");
            return;
        }

        Setting.EnableAntiKick = !Setting.EnableAntiKick;
        Setting.EnableInvisible = Setting.EnableAntiKick;
    }
}
