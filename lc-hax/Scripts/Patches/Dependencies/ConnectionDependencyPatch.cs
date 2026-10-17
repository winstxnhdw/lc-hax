using System;
using HarmonyLib;

[HarmonyPatch]
sealed class ConnectionDependencyPatch {
    internal static event Action? OnConnectionAttempt;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.SteamFriends_OnGameLobbyJoinRequested))]
    static void InvitePrefix() {
        if (Helper.FindObject<MenuManager>() is null) return;
        ConnectionDependencyPatch.OnConnectionAttempt?.Invoke();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.JoinLobby))]
    static void JoinLobbyPrefix() => ConnectionDependencyPatch.OnConnectionAttempt?.Invoke();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LobbySlot), nameof(LobbySlot.JoinLobbyAfterVerifying))]
    static void VerifyLobbyPrefix() => ConnectionDependencyPatch.OnConnectionAttempt?.Invoke();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.StartClient))]
    static void StartClientPrefix() => ConnectionDependencyPatch.OnConnectionAttempt?.Invoke();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.StartHost))]
    static void StartHostPrefix() => ConnectionDependencyPatch.OnConnectionAttempt?.Invoke();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MenuManager), nameof(MenuManager.StartAClient))]
    static void MenuClientPrefix() => ConnectionDependencyPatch.OnConnectionAttempt?.Invoke();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MenuManager), nameof(MenuManager.StartHosting))]
    static void MenuHostPrefix() => ConnectionDependencyPatch.OnConnectionAttempt?.Invoke();
}
