using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

sealed class AntiKickMod : MonoBehaviour {
    static Task? PendingRejoin { get; set; }

    bool WatchingLobby { get; set; }
    bool Rejoining { get; set; }
    bool IsRejoining => this.Rejoining && Setting.EnableAntiKick && !State.DisconnectedVoluntarily;

    void OnEnable() {
        InputListener.OnBackslashPress += this.ToggleAntiKick;
        GameListener.OnGameStart += this.OnGameStart;
        ConnectionDependencyPatch.OnConnectionAttempt += this.CancelRejoin;
    }

    void OnDisable() {
        InputListener.OnBackslashPress -= this.ToggleAntiKick;
        GameListener.OnGameStart -= this.OnGameStart;
        ConnectionDependencyPatch.OnConnectionAttempt -= this.CancelRejoin;
        this.CancelRejoin();
    }

    async void LateUpdate() {
        if (!Setting.EnableAntiKick) return;
        if (!this.WatchingLobby) return;
        if (AntiKickMod.PendingRejoin is { IsCompleted: false }) return;
        if (Helper.GameNetworkManager is not { isHostingGame: false, gameHasStarted: false, currentLobby: Lobby lobby } manager) return;
        if (Helper.StartOfRound is not { mostRecentlyJoinedClient: false, travellingToNewLevel: false }) return;
        if (Helper.FindObject<StartMatchLever>() is not { leverHasBeenPulled: false }) return;

        this.WatchingLobby = false;
        this.Rejoining = true;
        AntiKickMod.PendingRejoin = this.RejoinLobby(lobby, manager);
        await AntiKickMod.PendingRejoin;
    }

    async Task RejoinLobby(Lobby lobby, GameNetworkManager manager) {
        manager.Disconnect();
        RoomEnter result = await lobby.Join();

        if (result != RoomEnter.Success) {
            this.Rejoining = false;
            return;
        }

        await Helper.WaitUntil(
            () => !this.IsRejoining || (Helper.FindObject<MenuManager>() is not null && !manager.currentLobby.HasValue)
        );

        if (!this.IsRejoining) {
            this.Rejoining = false;
            return;
        }

        this.Rejoining = false;
        string lobbyName = lobby.GetData("name");
        SteamId host = lobby.Owner.Id;
        manager.currentLobby = lobby;
        manager.steamLobbyName = lobbyName;
        manager.StartClient(host);
    }

    void CancelRejoin() {
        this.WatchingLobby = false;
        this.Rejoining = false;
    }

    void OnGameStart() {
        this.Rejoining = false;
        this.WatchingLobby = true;
    }

    void ToggleAntiKick() {
        if (Helper.LocalPlayer is not null) {
            Chat.Print("You cannot toggle anti-kick while in-game!");
            return;
        }

        Setting.EnableAntiKick = !Setting.EnableAntiKick;

        if (!Setting.EnableAntiKick) {
            this.CancelRejoin();
        }
    }
}
