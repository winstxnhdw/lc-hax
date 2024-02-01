using Hax;
using UnityEngine;
using Steamworks;
using System.Collections;

public sealed class DisconnectMod : MonoBehaviour {
    bool IsShiftHeld { get; set; } = false;
    bool HasAnnouncedGameJoin { get; set; } = false;

    void OnEnable() {
        InputListener.onShiftButtonHold += this.HoldShift;
        InputListener.onF4Press += this.TryToDisconnect;
        InputListener.onF3Press += this.Rejoin;
    }

    void OnDisable() {
        InputListener.onShiftButtonHold -= this.HoldShift;
        InputListener.onF4Press -= this.TryToDisconnect;
        InputListener.onF3Press -= this.Rejoin;
    }

    void HoldShift(bool isHeld) => this.IsShiftHeld = isHeld;

    void TryToDisconnect() {
        if (!this.IsShiftHeld) return;

        GameNetworkManager.Instance.Disconnect();
        Setting.DisconnectedVoluntarily = true;
    }

    void Rejoin() {
        if (!this.IsShiftHeld) return;
        GameNetworkManager.Instance.Disconnect();
        Setting.DisconnectedVoluntarily = true;
        _ = this.StartCoroutine(this.JoinLastGame());
    }

    IEnumerator JoinLastGame() {
        yield return new WaitForSeconds(0.2f);
        this.HasAnnouncedGameJoin = false;
        if (Setting.ConnectedLobbyId is SteamId lobbyId) {
            GameNetworkManager.Instance.StartClient(lobbyId);
        }
    }
}
