using System.Threading.Tasks;
using GameNetcodeStuff;
using HarmonyLib;
using Steamworks;
using Steamworks.Data;

[HarmonyPatch]
sealed class AntiKickPatch {
    static Task? PendingRejoin { get; set; }
    static bool EventsSubscribed { get; set; }
    static bool WatchingLobby { get; set; }
    static bool Rejoining { get; set; }
    static bool IsRejoining => AntiKickPatch.Rejoining && Globals.EnableAntiKick && !State.DisconnectedVoluntarily;

    [HarmonyPrepare]
    static bool Prepare() {
        if (AntiKickPatch.EventsSubscribed) return true;

        InputListener.OnBackslashPress += AntiKickPatch.ToggleAntiKick;
        GameListener.OnGameStart += AntiKickPatch.OnGameStart;
        ConnectionDependencyPatch.OnConnectionAttempt += AntiKickPatch.CancelRejoin;
        AntiKickPatch.EventsSubscribed = true;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SendNewPlayerValuesServerRpc))]
    static bool Prefix(PlayerControllerB __instance) {
        if (!AntiKickPatch.IsLocalClient(__instance)) return true;
        if (Helper.GameNetworkManager?.currentLobby is not { } lobby) return true;

        AntiKickPatch.SetIdentity(__instance, SteamClient.SteamId, SteamClient.Name);
        AntiKickPatch.SetIdentity(__instance.playersManager.allPlayerScripts[0], lobby.Owner.Id, lobby.Owner.Name);

        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SendNewPlayerValuesClientRpc))]
    static void Postfix() {
        if (Helper.LocalPlayer is not { } localPlayer) return;
        if (!AntiKickPatch.IsLocalClient(localPlayer)) return;

        AntiKickPatch.SetIdentity(localPlayer, SteamClient.SteamId, SteamClient.Name);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.LateUpdate))]
    static async void RejoinWhenReady(StartOfRound __instance) {
        if (!Globals.EnableAntiKick) return;
        if (!AntiKickPatch.WatchingLobby) return;
        if (AntiKickPatch.PendingRejoin is { IsCompleted: false }) return;
        if (Helper.GameNetworkManager is not { isHostingGame: false, gameHasStarted: false, currentLobby: Lobby lobby } manager) return;
        if (__instance is not { mostRecentlyJoinedClient: false, travellingToNewLevel: false }) return;
        if (Helper.FindObject<StartMatchLever>() is not { leverHasBeenPulled: false }) return;

        AntiKickPatch.WatchingLobby = false;
        AntiKickPatch.Rejoining = true;
        AntiKickPatch.PendingRejoin = AntiKickPatch.RejoinLobby(lobby, manager);
        await AntiKickPatch.PendingRejoin;
    }

    static async Task RejoinLobby(Lobby lobby, GameNetworkManager manager) {
        manager.Disconnect();
        RoomEnter result = await lobby.Join();

        if (result != RoomEnter.Success) {
            AntiKickPatch.Rejoining = false;
            return;
        }

        await Helper.WaitUntil(
            () => !AntiKickPatch.IsRejoining || (Helper.FindObject<MenuManager>() is not null && !manager.currentLobby.HasValue)
        );

        if (!AntiKickPatch.IsRejoining) {
            AntiKickPatch.Rejoining = false;
            return;
        }

        AntiKickPatch.Rejoining = false;
        string lobbyName = lobby.GetData("name");
        SteamId host = lobby.Owner.Id;
        manager.currentLobby = lobby;
        manager.steamLobbyName = lobbyName;
        manager.StartClient(host);
    }

    static void CancelRejoin() {
        AntiKickPatch.WatchingLobby = false;
        AntiKickPatch.Rejoining = false;
    }

    static void OnGameStart() {
        AntiKickPatch.Rejoining = false;
        AntiKickPatch.WatchingLobby = true;
    }

    static void ToggleAntiKick() {
        if (Helper.LocalPlayer is not null) {
            Chat.Print("You cannot toggle anti-kick while in-game!");
            return;
        }

        Globals.EnableAntiKick = !Globals.EnableAntiKick;

        if (!Globals.EnableAntiKick) {
            AntiKickPatch.CancelRejoin();
        }
    }

    static bool IsLocalClient(PlayerControllerB player) =>
        Globals.EnableAntiKick &&
        player is { IsServer: false, IsOwner: true } &&
        player == Helper.LocalPlayer &&
        Helper.GameNetworkManager is { disableSteam: false } &&
        SteamClient.IsValid &&
        SteamClient.SteamId.Value != 0;

    static void SetIdentity(PlayerControllerB player, ulong steamId, string name) {
        player.playerSteamId = steamId;
        player.playerUsername = name;
        player.usernameBillboardText.text = name;
        player.playersManager.Unfake()?.mapScreen.Unfake()?.ChangeNameOfTargetTransform(player.transform, name);
        player.quickMenuManager.AddUserToPlayerList(steamId, name, player.PlayerIndex());
    }
}
