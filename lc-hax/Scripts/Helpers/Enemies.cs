using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace Hax;

internal static partial class Helper {
    internal static HashSet<EnemyAI> Enemies { get; } = Helper.StartOfRound?.inShipPhase is not false ? [] :
        Helper.FindObjects<EnemyAI>()
              .WhereIsNotNull()
              .Where(enemy => enemy.IsSpawned)
              .ToHashSet();

    internal static T? GetEnemy<T>() where T : EnemyAI =>
        Helper.Enemies.First(enemy => enemy is T) is T enemy ? enemy : null;

    internal static void Kill(this EnemyAI enemyInstance, ulong actualClientId) {
        if (enemyInstance is NutcrackerEnemyAI nutcracker) {
            nutcracker.ChangeEnemyOwnerServerRpc(actualClientId);
            nutcracker.KillEnemy();
        }

        enemyInstance.KillEnemyServerRpc(true);
    }

    internal static void Kill(EnemyAI enemyInstance) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        enemyInstance.Kill(localPlayer.actualClientId);
    }

    internal static bool IsBehaviourState(this EnemyAI enemyInstance, Enum state) =>
        enemyInstance.currentBehaviourStateIndex == Convert.ToInt32(state);

    internal static void SetBehaviourState(this EnemyAI enemyInstance, Enum state) {
        if (enemyInstance.IsBehaviourState(state)) return;
        enemyInstance.SwitchToBehaviourServerRpc(Convert.ToInt32(state));
    }

    internal static GrabbableObject? FindNearbyItem(this EnemyAI instance, float grabRange = 1f) {
        foreach (Collider collider in Physics.OverlapSphere(instance.transform.position, grabRange)) {
            if (!collider.TryGetComponent(out GrabbableObject item)) continue;
            if (!item.TryGetComponent(out NetworkObject _)) continue;

            return item;
        }

        return null;
    }
}
