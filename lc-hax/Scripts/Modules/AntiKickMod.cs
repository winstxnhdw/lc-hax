using UnityEngine;
using Hax;
using Steamworks;
using System.Collections;

public sealed class AntiKickMod : MonoBehaviour {
    bool HasGameStarted { get; set; } = false;
    bool HasAnnouncedGameJoin { get; set; } = false;
    bool ClearChatExecuted = false;

    void OnEnable() {
        InputListener.onBackslashPress += this.ToggleAntiKick;
        GameListener.onGameEnd += this.OnGameEnd;
    }

    void OnDisable() {
        InputListener.onBackslashPress -= this.ToggleAntiKick;
        GameListener.onGameEnd -= this.OnGameEnd;
    }

    void OnGameEnd() {
        this.HasAnnouncedGameJoin = false;

        if (!DisconnectMod.DisconnectionAttempted && !Setting.DisconnectedVoluntarily && Setting.EnableAntiKick && Setting.ConnectedLobbyId is SteamId lobbyId) {
            GameNetworkManager.Instance.StartClient(lobbyId);
            ClearChatExecuted = true;
            StartCoroutine(DelayClearChatToFalse());
        }
        DisconnectMod.DisconnectionAttempted = false;
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

        if (!ClearChatExecuted) {
            Helper.CreateComponent<WaitForBehaviour>().SetPredicate(time => time > 1.0f).Init(() => {
                Chat.Announce($"{Helper.LocalPlayer?.playerUsername} disconnected.", true);
                Chat.Print("You are invisible! Do /invis to disable!");
            });
        }

        if (ClearChatExecuted) {
            ClearChatExecuted = false;
            Helper.CreateComponent<WaitForBehaviour>().SetPredicate(time => time > 1.0f).Init(() => {
                Chat.Clear();
            });
        }
    }

    IEnumerator DelayClearChatToFalse() {
        yield return new WaitForSeconds(6.5f);
        ClearChatExecuted = false;
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
