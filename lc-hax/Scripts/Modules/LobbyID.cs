using UnityEngine;
using Steamworks;
using Hax;

public class LobbyID : MonoBehaviour {
    bool IsShiftHeld { get; set; } = false;

    void OnEnable() {
        InputListener.onShiftButtonHold += HoldShift;
        InputListener.onCPress += CopyToClipboard;
        InputListener.onVPress += PasteFromClipboard;
    }

    void OnDisable() {
        InputListener.onShiftButtonHold -= HoldShift;
        InputListener.onCPress -= CopyToClipboard;
        InputListener.onVPress -= PasteFromClipboard;
    }

    void HoldShift(bool isHeld) => IsShiftHeld = isHeld;

    void CopyToClipboard() {
        if (!IsShiftHeld || Setting.ConnectedLobbyId == null) return;
        GUIUtility.systemCopyBuffer = Setting.ConnectedLobbyId.ToString();
    }

    void PasteFromClipboard() {
        if (!IsShiftHeld) return;
        string clipboardText = GUIUtility.systemCopyBuffer;
        if (ulong.TryParse(clipboardText, out ulong lobbyIdValue)) {
            SteamId lobbyId = new SteamId { Value = lobbyIdValue };
            Setting.ConnectedLobbyId = lobbyId;
        }
    }
}
