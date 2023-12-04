#pragma warning disable IDE1006

using HarmonyLib;
using Unity.Netcode;

namespace Hax;

public static class AntiDisconnectPatch {
    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    [HarmonyPrefix]
    static bool Disconnect() => true;

    [HarmonyPatch(typeof(GameNetworkManager), "Singleton_OnClientDisconnectCallback")]
    [HarmonyPrefix]
    static bool Singleton_OnClientDisconnectCallback() => true;

    [HarmonyPatch(typeof(NetworkConnectionManager), "OnClientDisconnectFromServer")]
    [HarmonyPrefix]
    static bool OnClientDisconnectFromServer() => true;

    [HarmonyPatch(typeof(NetworkConnectionManager), "DisconnectEventHandler")]
    [HarmonyPrefix]
    static bool DisconnectEventHandler() => true;

    [HarmonyPatch(typeof(StartOfRound), "OnPlayerDC")]
    [HarmonyPrefix]
    static bool OnPlayerDC() => true;
}
