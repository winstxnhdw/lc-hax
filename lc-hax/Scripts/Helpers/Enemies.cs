using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hax;

internal static partial class Helper {
    internal static HashSet<EnemyAI> Enemies { get; } = Helper.StartOfRound?.inShipPhase is not false ? [] :
        Helper.FindObjects<EnemyAI>()
              .WhereIsNotNull()
              .Where(enemy => enemy.IsSpawned)
              .ToHashSet();

    internal static T? GetEnemy<T>() where T : EnemyAI =>
        Helper.Enemies.First(enemy => enemy is T) is T enemy ? enemy : null;

    internal static bool IsBehaviourState(this EnemyAI enemyInstance, Enum state) =>
        enemyInstance.currentBehaviourStateIndex == Convert.ToInt32(state);

    internal static void SetBehaviourState(this EnemyAI enemyInstance, Enum state) {
        if (enemyInstance.IsBehaviourState(state)) return;
        enemyInstance.SwitchToBehaviourServerRpc(Convert.ToInt32(state));
    }

    internal static PlayerControllerB? GetPlayerAboutToKilledByEnemy(int playerObjectID) {
        PlayerControllerB[] players = Helper.Players;
        return players.First(player => (int)player.playerClientId == playerObjectID);
    }

    internal static bool IsHostileEnemy(EnemyType enemy) =>
        !enemy.enemyName.Contains("Docile Locust Bees", StringComparison.OrdinalIgnoreCase) ||
        !enemy.enemyName.Contains("Manticoil", StringComparison.OrdinalIgnoreCase);

    internal static Dictionary<string, GameObject> HostileEnemies { get; } =
        Resources.FindObjectsOfTypeAll<EnemyType>()
            .Where(Helper.IsHostileEnemy)
            .GroupBy(enemy => enemy.enemyName)
            .ToDictionary(enemyGroup => enemyGroup.Key, enemy => Enumerable.First(enemy).enemyPrefab);

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


    internal static GrabbableObject? FindNearbyItem(this EnemyAI instance, float grabRange = 1f) {
        foreach (Collider collider in Physics.OverlapSphere(instance.transform.position, grabRange)) {
            if (!collider.TryGetComponent(out GrabbableObject item)) continue;
            if (!item.TryGetComponent(out NetworkObject _)) continue;

            return item;
        }

        return null;
    }
}
