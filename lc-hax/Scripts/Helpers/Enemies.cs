using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hax;

internal static partial class Helper {
    internal static HashSet<EnemyAI> Enemies { get; } = Helper.StartOfRound is { inShipPhase: true }
        ? []
        : Helper.FindObjects<EnemyAI>()
            .WhereIsNotNull()
            .Where(enemy => enemy.IsSpawned)
            .ToHashSet();

    internal static bool IsHostileEnemy(EnemyType enemy) =>
        !enemy.enemyName.Contains("Docile Locust Bees", StringComparison.InvariantCultureIgnoreCase) &&
        !enemy.enemyName.Contains("Manticoil", StringComparison.InvariantCultureIgnoreCase);

    internal static Dictionary<string, GameObject> HostileEnemies { get; } =
        Resources.FindObjectsOfTypeAll<EnemyType>()
            .Where(Helper.IsHostileEnemy)
            .GroupBy(enemy => enemy.enemyName)
            .ToDictionary(enemyGroup => enemyGroup.Key, enemy => Enumerable.First(enemy).enemyPrefab);

    internal static T? GetEnemy<T>() where T : EnemyAI =>
        Helper.Enemies.First(enemy => enemy is T) is T enemy ? enemy : null;

    internal static void Kill(this EnemyAI enemy, ulong actualClientId) {
        enemy.ChangeEnemyOwnerServerRpc(actualClientId);

        if (enemy is NutcrackerEnemyAI nutcracker) {
            nutcracker.KillEnemy();
        }

        else {
            enemy.KillEnemyServerRpc(true);
        }
    }

    internal static bool SetOutsideStatus(this EnemyAI enemy, bool isOutside) {
        if (enemy.isOutside == isOutside) return false;

        enemy.isOutside = isOutside;
        enemy.allAINodes = GameObject.FindGameObjectsWithTag(enemy.isOutside ? "OutsideAINode" : "AINode");
        Transform closestNodePos = enemy.ChooseClosestNodeToPosition(enemy.transform.position, false, 0);
        _ = enemy.SetDestinationToPosition(closestNodePos.position, true);
        _ = enemy.agent.Warp(enemy.transform.position);
        enemy.SyncPositionToClients();
        enemy.agent.ResetPath();
        enemy.EnableEnemyMesh(true, false);
        enemy.StopSearch(enemy.currentSearch, true);
        return true;
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

    internal static void SpawnEnemies(Vector3 position, GameObject prefab, ulong amount = 1) {
        for (ulong i = 0; i < amount; i++) {
            _ = Helper.SpawnEnemy(position, prefab);
        }
    }

    internal static EnemyAI? SpawnEnemy(Vector3 position, GameObject prefab) {
        if (prefab == null) return null;
        GameObject enemy = Object.Instantiate(prefab, position, Quaternion.identity);

        if (!enemy.TryGetComponent(out NetworkObject networkObject)) {
            Object.Destroy(enemy);
            return null;
        }

        networkObject.Spawn(true);
        // get the enemy ai component
        if (!enemy.TryGetComponent(out EnemyAI enemyAI)) {
            Object.Destroy(enemy);
            return null;
        }

        return enemyAI;
    }

    internal static bool IsLocalPlayerAboutToGetKilledByEnemy(this EnemyAI instance, Collider other) {
        if (instance == null) return false;
        if (other == null) return false;
        PlayerControllerB playerControllerB = instance.MeetsStandardPlayerCollisionConditions(other, false, false);
        return playerControllerB != null && playerControllerB.IsSelf();
    }


    internal static bool IsLocalPlayerAboutToGetKilledByEnemy(int PlayerID) {
        PlayerControllerB? player = Helper.GetPlayerAboutToKilledByEnemy(PlayerID);
        return player != null && player.IsSelf();
    }

    internal static PlayerControllerB? GetPlayerAboutToKilledByEnemy(int playerObjectID) {
        PlayerControllerB[] players = Helper.Players;
        return players.First(player => (int)player.playerClientId == playerObjectID);
    }
}
