using GameNetcodeStuff;
using Hax;
using UnityEngine;

internal sealed class KillClickMod : MonoBehaviour
{
    private RaycastHit[] RaycastHits { get; } = new RaycastHit[100];

    private void OnEnable()
    {
        InputListener.OnLeftButtonPress += Kill;
    }

    private void OnDisable()
    {
        InputListener.OnLeftButtonPress -= Kill;
    }

    private void Kill()
    {
        if (!Setting.EnableKillOnLeftClick) return;
        if (Helper.CurrentCamera is not Camera camera) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        for (var i = 0; i < RaycastHits.SphereCastForward(camera.transform); i++)
        {
            if (!RaycastHits[i].collider.TryGetComponent(out EnemyAICollisionDetect enemy)) continue;

            enemy.mainScript.TakeOwnerShipIfNotOwned();
            enemy.mainScript.Kill();
            break;
        }
    }
}