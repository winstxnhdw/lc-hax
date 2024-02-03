using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static HashSet<EnemyAI> Enemies { get; } = [];

    public static T? GetEnemy<T>() where T : EnemyAI =>
        Helper.Enemies.First(enemy => enemy is T) is T enemy ? enemy : null;

    public static GrabbableObject? FindNearbyItem(this EnemyAI instance, float grabRange = 1f) {
        Collider[] Search = Physics.OverlapSphere(instance.transform.position, grabRange);
        for (int i = 0; i < Search.Length; i++) {
            if (!Search[i].TryGetComponent(out GrabbableObject item)) continue;
            if (item.TryGetComponent(out NetworkObject _)) return item;
        }

        return null;
    }
}
