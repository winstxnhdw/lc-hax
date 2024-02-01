using UnityEngine;
using Steamworks;
using Hax;

public class LobbyID : MonoBehaviour {
    bool IsShiftHeld { get; set; } = false;

    void OnEnable() {
        InputListener.onShiftButtonHold += this.HoldShift;
        InputListener.onCPress += this.CopyToClipboard;
        InputListener.onVPress += this.PasteFromClipboard;
    }

    void OnDisable() {
        InputListener.onShiftButtonHold -= this.HoldShift;
        InputListener.onCPress -= this.CopyToClipboard;
        InputListener.onVPress -= this.PasteFromClipboard;
    }

    void HoldShift(bool isHeld) => this.IsShiftHeld = isHeld;

    void CopyToClipboard() {
        if (!this.IsShiftHeld || Setting.ConnectedLobbyId == null) return;
        GUIUtility.systemCopyBuffer = Setting.ConnectedLobbyId.ToString();
    }

    void PasteFromClipboard() {
        if (!this.IsShiftHeld) return;
        string clipboardText = GUIUtility.systemCopyBuffer;
        if (ulong.TryParse(clipboardText, out ulong lobbyIdValue)) {
            SteamId lobbyId = new() { Value = lobbyIdValue };
            Setting.ConnectedLobbyId = lobbyId;
        }
    }
}
