using UnityEngine;
using GameNetcodeStuff;

sealed class KillClickMod : MonoBehaviour {
    void OnEnable() => InputListener.OnLeftButtonPress += this.Kill;

    void OnDisable() => InputListener.OnLeftButtonPress -= this.Kill;

    void Kill() {
        if (!Setting.EnableKillOnLeftClick) return;
        if (Helper.CurrentCamera is not { } camera) return;
        if (Helper.LocalPlayer is not { } localPlayer) return;

        foreach (RaycastHit hit in camera.transform.SphereCastForward()) {
            if (!hit.collider.TryGetComponent(out EnemyAICollisionDetect enemy)) continue;

            enemy.mainScript.ChangeOwnershipOfEnemy(localPlayer.actualClientId);
            enemy.mainScript.Kill(localPlayer.actualClientId);
            break;
        }
    }
}
