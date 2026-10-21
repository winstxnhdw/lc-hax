using System.Text.RegularExpressions;
using HarmonyLib;
using Steamworks;
using UnityEngine;

[HarmonyPatch(typeof(InputListener), "Update")]
sealed class DisconnectPatch {
    static bool EventsSubscribed { get; set; }
    static bool IsShiftHeld { get; set; }

    static void Prefix() {
        if (DisconnectPatch.EventsSubscribed) return;

        InputListener.OnShiftButtonHold += DisconnectPatch.HoldShift;
        InputListener.OnF4Press += DisconnectPatch.TryToDisconnect;
        InputListener.OnF5Press += DisconnectPatch.TryToConnect;
        DisconnectPatch.EventsSubscribed = true;
    }

    static void HoldShift(bool isHeld) => DisconnectPatch.IsShiftHeld = isHeld;

    static SteamId? GetSteamIdFromClipboard() {
        string clipboardText = GUIUtility.systemCopyBuffer;

        return !string.IsNullOrWhiteSpace(clipboardText) && Regex.IsMatch(clipboardText, @"^\d{17}$")
            ? new SteamId { Value = ulong.Parse(clipboardText) }
            : null;
    }

    static void TryToDisconnect() {
        if (!DisconnectPatch.IsShiftHeld) return;
        if (Helper.LocalPlayer is null) return;

        Helper.GameNetworkManager?.Disconnect();
        State.DisconnectedVoluntarily = true;
    }

    static void TryToConnect() {
        if (!DisconnectPatch.IsShiftHeld) return;

        if (DisconnectPatch.GetSteamIdFromClipboard() is SteamId playerSteamId) {
            Helper.GameNetworkManager?.StartClient(playerSteamId);
        }

        else if (State.ConnectedLobby is ConnectedLobby connectedLobby) {
            Helper.GameNetworkManager?.JoinLobby(connectedLobby.Lobby, connectedLobby.SteamId);
        }
    }
}
