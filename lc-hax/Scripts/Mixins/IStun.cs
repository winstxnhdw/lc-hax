using GameNetcodeStuff;
using Hax;
using UnityEngine;

interface IStun { }

static class StunMixin {
    internal static void Stun(this IStun _, Vector3 position, float radius, float duration = 1.0f) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        Physics.OverlapSphere(position, radius, 524288)
               .ForEach(collider => {
                   if (!collider.TryGetComponent(out EnemyAICollisionDetect enemy)) return;
                   enemy.mainScript.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
                   enemy.mainScript.SetEnemyStunned(true, duration);
               });
    }
}
