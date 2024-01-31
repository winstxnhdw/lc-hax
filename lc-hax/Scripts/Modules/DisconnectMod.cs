using UnityEngine;

public sealed class DisconnectMod : MonoBehaviour {
    bool IsShiftHeld { get; set; } = false;

    void OnEnable() {
        InputListener.onShiftButtonHold += this.HoldShift;
        InputListener.onF4Press += this.TryToDisconnect;
    }

    void OnDisable() {
        InputListener.onShiftButtonHold -= this.HoldShift;
        InputListener.onF4Press -= this.TryToDisconnect;
    }

    void HoldShift(bool isHeld) => this.IsShiftHeld = isHeld;

    void TryToDisconnect() {
        if (!this.IsShiftHeld) return;
        GameNetworkManager.Instance.Disconnect();
        DisconnectionAttempted = true;
    }
}
