#region

using System.Collections;
using hax;
using Hax;
using UnityEngine;

#endregion

sealed class AntiKickMod : MonoBehaviour {
    AntiKickMod Instance;

    void Awake() {
        if (this.Instance != null) {
            Destroy(this);
            return;
        }

        this.Instance = this;
        this.UpdateLabel();
    }

    void DisableLabel() => MinimalGUIHelper.Remove("Anti-Kick");

    void UpdateLabel() => MinimalGUIHelper.AddText("Anti-Kick",
        $"Anti-Kick: {(Setting.EnableAntiKick ? "<color=green>On</color>" : "<color=red>Off</color>")}");

    void OnEnable() {
        InputListener.OnBackslashPress += this.ToggleAntiKick;
        GameListener.OnGameStart += this.OnGameStart;
        GameListener.OnGameEnd += this.OnGameEnd;
        this.UpdateLabel();
    }

    void OnDisable() {
        InputListener.OnBackslashPress -= this.ToggleAntiKick;
        GameListener.OnGameStart -= this.OnGameStart;
        GameListener.OnGameEnd -= this.OnGameEnd;
        this.DisableLabel();
    }

    IEnumerator RejoinLobby() {
        if (State.ConnectedLobby is not ConnectedLobby connectedLobby) yield break;

        WaitForEndOfFrame waitForEndOfFrame = new();

        while (Helper.FindObject<MenuManager>() is null) yield return waitForEndOfFrame;

        while (Helper.GameNetworkManager?.currentLobby.HasValue is not false) yield return waitForEndOfFrame;

        Helper.GameNetworkManager?.JoinLobby(connectedLobby.Lobby, connectedLobby.SteamId);
    }

    void OnGameEnd() {
        if (State.DisconnectedVoluntarily || !Setting.EnableAntiKick) return;
        _ = this.StartCoroutine(this.RejoinLobby());
    }

    void OnGameStart() {
        if (!Setting.EnableAntiKick || !Setting.EnableInvisible) return;

        Chat.Clear();
        Helper.SendNotification(
            "Anti-kick",
            "You are invisible to other players! Do /invis to disable!",
            true
        );
    }

    void ToggleAntiKick() {
        if (Helper.LocalPlayer is not null) {
            Chat.Print("You cannot toggle anti-kick while in-game!");
            return;
        }

        this.UpdateLabel();
        Setting.EnableAntiKick = !Setting.EnableAntiKick;
        Setting.EnableInvisible = Setting.EnableAntiKick;
    }
}
