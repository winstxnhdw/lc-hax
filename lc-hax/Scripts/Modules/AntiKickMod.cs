using UnityEngine;
using Hax;
using Steamworks;

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

    void OnGameEnd() {
        if (!State.DisconnectedVoluntarily && Setting.EnableAntiKick && State.ConnectedLobbyId is SteamId lobbyId) {
            Helper.GameNetworkManager?.StartClient(lobbyId);
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
