using UnityEngine;
using Hax;
using System.Collections;

internal sealed class AntiKickMod : MonoBehaviour {
    bool HasGameStarted { get; set; } = false;
    bool HasAnnouncedGameJoin { get; set; } = false;

    void OnEnable() {
        InputListener.OnBackslashPress += this.ToggleAntiKick;
        GameListener.OnGameEnd += this.OnGameEnd;
    }

    void OnDisable() {
        InputListener.OnBackslashPress -= this.ToggleAntiKick;
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

    void OnGameEnd() {
        if (!State.DisconnectedVoluntarily && Setting.EnableAntiKick) {
            _ = this.StartCoroutine(this.RejoinLobby());
        }

        this.HasAnnouncedGameJoin = false;
    }

    bool FindJoinMessageInHistory() =>
        Helper.HUDManager?.ChatMessageHistory.First(message =>
            message.Contains($"{Helper.LocalPlayer?.playerUsername} joined the ship.")
        ) is not null;

    void Update() {
        if (!Setting.EnableAntiKick) return;
        if (this.HasAnnouncedGameJoin) return;
        if (!this.FindJoinMessageInHistory()) return;

        this.HasAnnouncedGameJoin = true;

        Helper.ShortDelay(() => {
            Chat.Clear();
            Chat.Print("You are invisible! Do /invis to disable!");
        });
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
