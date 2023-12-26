using System.Linq;
using UnityEngine;

namespace Hax;

public interface IStun { }

public static class IStunMixin {
    public static void Stun(this IStun _, Vector3 position, float radius, float duration = 1.0f) =>
        Physics.OverlapSphere(position, radius, 524288)
               .Select(collider => collider.GetComponent<EnemyAICollisionDetect>())
               .Where(enemy => enemy != null)
               .ForEach(enemy => enemy.mainScript.SetEnemyStunned(true, duration));
}
