using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace Hax;

static partial class Helper {
    internal static HashSet<EnemyAI> Enemies { get; } = Helper.StartOfRound is not { inShipPhase: false } ? [] :
        Helper.FindObjects<EnemyAI>()
              .WhereIsNotNull()
              .Where(enemy => enemy.IsSpawned)
              .ToHashSet();

    internal static T? GetEnemy<T>() where T : EnemyAI => Helper.Enemies.First(enemy => enemy is T) as T;

    internal static void Kill(this EnemyAI enemy, ulong actualClientId) {
        enemy.ChangeEnemyOwnerServerRpc(actualClientId);

        if (enemy is NutcrackerEnemyAI nutcracker) {
            nutcracker.KillEnemy();
        }

        else {
            enemy.KillEnemyServerRpc(true);
        }
    }

    internal static void SetOutsideStatus(this EnemyAI enemy, bool isOutside) {
        if (enemy.isOutside == isOutside) return;

        enemy.isOutside = isOutside;
        enemy.allAINodes = GameObject.FindGameObjectsWithTag(enemy.isOutside ? "OutsideAINode" : "AINode");
    }

    internal static void Kill(EnemyAI enemy) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        enemy.Kill(localPlayer.actualClientId);
    }

    internal static bool IsBehaviourState(this EnemyAI enemy, Enum state) =>
        enemy.currentBehaviourStateIndex == Convert.ToInt32(state);

    internal static void SetBehaviourState(this EnemyAI enemy, Enum state) {
        if (enemy.IsBehaviourState(state)) return;
        enemy.SwitchToBehaviourServerRpc(Convert.ToInt32(state));
    }

    internal static GrabbableObject? FindNearbyItem(this EnemyAI enemy, float grabRange = 1.0f) {
        foreach (Collider collider in Physics.OverlapSphere(enemy.transform.position, grabRange)) {
            if (!collider.TryGetComponent(out GrabbableObject item)) continue;
            if (!item.TryGetComponent(out NetworkObject _)) continue;

            return item;
        }

        return null;
    }
}
