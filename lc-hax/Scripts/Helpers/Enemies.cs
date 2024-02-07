using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Hax;

internal static partial class Helper {
    internal static HashSet<EnemyAI> Enemies { get; } = [];

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

    internal static bool IsLocalPlayerAboutToGetKilledByEnemy(int PlayerID) {
        PlayerControllerB? player = Helper.GetPlayerAboutToKilledByEnemy(PlayerID);
        return player != null && player.IsSelf();
    }

    internal static bool IsLocalPlayerAboutToGetKilledByEnemy(this EnemyAI instance, Collider other) {
        if (instance == null) return false;
        if (other == null) return false;
        PlayerControllerB playerControllerB = instance.MeetsStandardPlayerCollisionConditions(other, false, false);
        return playerControllerB != null && playerControllerB.IsSelf();
    }

    internal static Dictionary<string, GameObject> SpawnableEnemies {
        get {
            if (Helper.RoundManager == null) return [];
            if (Helper.RoundManager.currentLevel == null) return [];
            Dictionary<string, GameObject> result = [];
            foreach (SpawnableEnemyWithRarity enemy in Helper.RoundManager.currentLevel.Enemies) {
                string name = enemy.enemyType.enemyName;
                GameObject prefab = enemy.enemyType.enemyPrefab;
                if (!result.ContainsKey(name)) {
                    result.Add(name, prefab);
                }
            }
            foreach (SpawnableEnemyWithRarity enemy in Helper.RoundManager.currentLevel.OutsideEnemies) {
                string name = enemy.enemyType.enemyName;
                GameObject prefab = enemy.enemyType.enemyPrefab;
                if (!result.ContainsKey(name)) {
                    result.Add(name, prefab);
                }
            }
            foreach (SpawnableEnemyWithRarity enemy in Helper.RoundManager.currentLevel.DaytimeEnemies) {
                string name = enemy.enemyType.enemyName;
                GameObject prefab = enemy.enemyType.enemyPrefab;
                if (!result.ContainsKey(name)) {
                    result.Add(name, prefab);
                }
            }
            return result;
        }
    }

    internal static GrabbableObject? FindNearbyItem(this EnemyAI instance, float grabRange = 1f) {
        foreach (Collider collider in Physics.OverlapSphere(instance.transform.position, grabRange)) {
            if (!collider.TryGetComponent(out GrabbableObject item)) continue;
            if (!item.TryGetComponent(out NetworkObject _)) continue;

            return item;
        }

        return null;
    }
    internal static void MouthDogChasePlayer(this MouthDogAI instance, PlayerControllerB player) {
    }
}
