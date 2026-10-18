using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

[HarmonyPatch]
sealed class ConnectionDependencyPatch {
    internal static event Action? OnConnectionAttempt;

    static IEnumerable<MethodBase> TargetMethods() => [
        AccessTools.Method(typeof(GameNetworkManager), nameof(GameNetworkManager.JoinLobby)),
        AccessTools.Method(typeof(LobbySlot), nameof(LobbySlot.JoinLobbyAfterVerifying)),
        AccessTools.Method(typeof(GameNetworkManager), nameof(GameNetworkManager.StartClient)),
        AccessTools.Method(typeof(GameNetworkManager), nameof(GameNetworkManager.StartHost)),
        AccessTools.Method(typeof(MenuManager), nameof(MenuManager.StartAClient)),
        AccessTools.Method(typeof(MenuManager), nameof(MenuManager.StartHosting))
    ];

    [HarmonyPrefix]
    internal static void NotifyConnectionAttempt() => ConnectionDependencyPatch.OnConnectionAttempt?.Invoke();
}

[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.SteamFriends_OnGameLobbyJoinRequested))]
sealed class ConnectionInvitePatch {
    static void Prefix() {
        if (Helper.FindObject<MenuManager>() is null) return;
        ConnectionDependencyPatch.NotifyConnectionAttempt();
    }
}
