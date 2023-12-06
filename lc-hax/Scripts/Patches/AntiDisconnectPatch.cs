#pragma warning disable IDE1006

using HarmonyLib;
using Unity.Netcode;

namespace Hax;

class AntiDisconnectPatch {
    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    [HarmonyPrefix]
    static bool Disconnect() => false;

    [HarmonyPatch(typeof(GameNetworkManager), "Singleton_OnClientDisconnectCallback")]
    [HarmonyPrefix]
    static bool Singleton_OnClientDisconnectCallback() => false;

    [HarmonyPatch(typeof(NetworkConnectionManager), "OnClientDisconnectFromServer")]
    [HarmonyPrefix]
    static bool OnClientDisconnectFromServer() => false;

    [HarmonyPatch(typeof(NetworkConnectionManager), "DisconnectEventHandler")]
    [HarmonyPrefix]
    static bool DisconnectEventHandler() => false;

    [HarmonyPatch(typeof(StartOfRound), "OnPlayerDC")]
    [HarmonyPrefix]
    static bool OnPlayerDC() => false;
}
