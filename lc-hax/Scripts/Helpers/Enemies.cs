using System;
using System.Collections.Generic;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using ZLinq;

static partial class Helper {
    internal static HashSet<EnemyAI> Enemies { get; } = Helper.StartOfRound is { inShipPhase: false }
        ? Helper.FindObjects<EnemyAI>().WhereIsNotNull().AsValueEnumerable().Where(enemy => enemy.IsSpawned).ToHashSet()
        : [];

    internal static T? GetEnemy<T>() where T : EnemyAI => Helper.Enemies.First(enemy => enemy is T) as T;

    internal static PlayerControllerB? GetClosestPlayer(EnemyAI enemy, float maxDistance = float.PositiveInfinity) {
        if (maxDistance < 0.0f) return null;

        PlayerControllerB? target = null;
        float closestDistance = float.PositiveInfinity;
        float maxDistanceSqr = maxDistance * maxDistance;
        Vector3 origin = enemy.transform.position;

        foreach (PlayerControllerB player in Helper.Players) {
            if (player is not { isPlayerControlled: true, isPlayerDead: false }) continue;
            if (player.IsSelf()) continue;

            float distance = (player.transform.position - origin).sqrMagnitude;

            if (distance > maxDistanceSqr || distance >= closestDistance) {
                continue;
            }

            closestDistance = distance;
            target = player;
        }

        return target;
    }

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
        enemy.SwitchToBehaviourState(Convert.ToInt32(state));
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
