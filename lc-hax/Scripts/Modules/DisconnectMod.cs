using System.Text.RegularExpressions;
using Steamworks;
using UnityEngine;

sealed class DisconnectMod : MonoBehaviour {
    bool IsShiftHeld { get; set; }

    void OnEnable() {
        InputListener.OnShiftButtonHold += this.HoldShift;
        InputListener.OnF4Press += this.TryToDisconnect;
        InputListener.OnF5Press += this.TryToConnect;
    }

    void OnDisable() {
        InputListener.OnShiftButtonHold -= this.HoldShift;
        InputListener.OnF4Press -= this.TryToDisconnect;
        InputListener.OnF5Press -= this.TryToConnect;
    }

    void HoldShift(bool isHeld) => this.IsShiftHeld = isHeld;

    static SteamId? GetSteamIdFromClipboard() {
        string clipboardText = GUIUtility.systemCopyBuffer;

        return !string.IsNullOrWhiteSpace(clipboardText) && Regex.IsMatch(clipboardText, @"^\d{17}$")
            ? new SteamId { Value = ulong.Parse(clipboardText) }
            : null;
    }

    void TryToDisconnect() {
        if (!this.IsShiftHeld) return;
        if (Helper.LocalPlayer is null) return;

        Helper.GameNetworkManager?.Disconnect();
        State.DisconnectedVoluntarily = true;
    }

    void TryToConnect() {
        if (!this.IsShiftHeld) return;
        if (DisconnectMod.GetSteamIdFromClipboard() is SteamId playerSteamId) {
            Helper.GameNetworkManager?.StartClient(playerSteamId);
        }

        else if (State.ConnectedLobby is ConnectedLobby connectedLobby) {
            Helper.GameNetworkManager?.JoinLobby(connectedLobby.Lobby, connectedLobby.SteamId);
        }
    }
}
