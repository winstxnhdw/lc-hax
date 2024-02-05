using Hax;
using UnityEngine;

public sealed class DisconnectMod : MonoBehaviour {
    bool IsShiftHeld { get; set; } = false;

    void OnEnable() {
        InputListener.OnShiftButtonHold += this.HoldShift;
        InputListener.OnF4Press += this.TryToDisconnect;
    }

    void OnDisable() {
        InputListener.OnShiftButtonHold -= this.HoldShift;
        InputListener.OnF4Press -= this.TryToDisconnect;
    }

    void HoldShift(bool isHeld) => this.IsShiftHeld = isHeld;

    void TryToDisconnect() {
        if (!this.IsShiftHeld) return;

        GameNetworkManager.Instance.Disconnect();
        Setting.DisconnectedVoluntarily = true;
    }
}
