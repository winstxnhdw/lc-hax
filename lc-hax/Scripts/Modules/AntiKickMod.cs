using System.Collections;
using UnityEngine;

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

    static IEnumerator RejoinLobby() {
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
        if (State.DisconnectedVoluntarily) return;
        if (!Setting.EnableAntiKick) return;

        _ = this.StartCoroutine(AntiKickMod.RejoinLobby());
    }

    void OnGameStart() {
        if (!Setting.EnableAntiKick) return;
        if (!Setting.EnableInvisible) return;

        Chat.Clear();
        Helper.SendNotification(
            title: "Anti-kick",
            body: "You are invisible to other players! Do /invis to disable!",
            isWarning: true
        );
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
