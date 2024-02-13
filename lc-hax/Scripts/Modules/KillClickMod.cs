using UnityEngine;
using Hax;
using GameNetcodeStuff;

internal sealed class KillClickMod : MonoBehaviour {
    RaycastHit[] RaycastHits { get; set; } = new RaycastHit[100];

    void OnEnable() => InputListener.OnLeftButtonPress += this.Kill;
    void OnDisable() => InputListener.OnLeftButtonPress -= this.Kill;

    bool IsHoldingADefensiveWeapon() =>
        Helper.LocalPlayer?.currentlyHeldObjectServer.itemProperties.isDefensiveWeapon is not false;

    void Kill() {
        if (!Setting.EnableKillOnLeftClick) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (Helper.CurrentCamera is not Camera camera) return;
        if (this.IsHoldingADefensiveWeapon()) return;

        this.RaycastHits.SphereCastForward(camera.transform).Range().ForEach(i => {
            if (!this.RaycastHits[i].collider.TryGetComponent(out EnemyAICollisionDetect enemy)) return;
            enemy.mainScript.Kill(localPlayer.actualClientId);
        });
    }
}
