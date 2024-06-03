#region

using GameNetcodeStuff;
using Hax;
using UnityEngine;

#endregion

sealed class KillClickMod : MonoBehaviour {
    RaycastHit[] RaycastHits { get; } = new RaycastHit[100];

    void OnEnable() => InputListener.OnLeftButtonPress += this.Kill;

    void OnDisable() => InputListener.OnLeftButtonPress -= this.Kill;

    void Kill() {
        if (!Setting.EnableKillOnLeftClick) return;
        if (Helper.CurrentCamera is not Camera camera) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        for (int i = 0; i < this.RaycastHits.SphereCastForward(camera.transform); i++) {
            if (!this.RaycastHits[i].collider.TryGetComponent(out EnemyAICollisionDetect enemy)) continue;

            enemy.mainScript.TakeOwnerShipIfNotOwned();
            enemy.mainScript.Kill();
            break;
        }
    }
}
