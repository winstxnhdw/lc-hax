using System.Text.RegularExpressions;
using Hax;
using Steamworks;
using UnityEngine;

internal sealed class DisconnectMod : MonoBehaviour
{
    private bool IsShiftHeld { get; set; } = false;

    private void OnEnable()
    {
        InputListener.OnShiftButtonHold += HoldShift;
        InputListener.OnF4Press += TryToDisconnect;
        InputListener.OnF5Press += TryToConnect;
    }

    private void OnDisable()
    {
        InputListener.OnShiftButtonHold -= HoldShift;
        InputListener.OnF4Press -= TryToDisconnect;
        InputListener.OnF5Press -= TryToConnect;
    }

    private void HoldShift(bool isHeld)
    {
        IsShiftHeld = isHeld;
    }

    private SteamId? GetSteamIdFromClipboard()
    {
        var clipboardText = GUIUtility.systemCopyBuffer;

        return !string.IsNullOrWhiteSpace(clipboardText) && Regex.IsMatch(clipboardText, @"^\d{17}$")
            ? new SteamId { Value = ulong.Parse(clipboardText) }
            : null;
    }

    private void TryToDisconnect()
    {
        if (!IsShiftHeld) return;
        if (Helper.LocalPlayer is null) return;

        Helper.GameNetworkManager?.Disconnect();
        State.DisconnectedVoluntarily = true;
    }

    private void TryToConnect()
    {
        if (!IsShiftHeld) return;
        if (GetSteamIdFromClipboard() is SteamId playerSteamId)
            Helper.GameNetworkManager?.StartClient(playerSteamId);

        else if (State.ConnectedLobby is ConnectedLobby connectedLobby)
            Helper.GameNetworkManager?.JoinLobby(connectedLobby.Lobby, connectedLobby.SteamId);
    }
}