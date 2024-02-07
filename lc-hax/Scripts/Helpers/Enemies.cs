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

    private static Dictionary<string, GameObject>? _AllSpawnableEnemies;

    internal static Dictionary<string, GameObject> AllSpawnableEnemies {
        get {
            if (_AllSpawnableEnemies == null) {
                _AllSpawnableEnemies = [];

                HashSet<(string Name, GameObject Prefab)> uniqueEnemies = [];

                SelectableLevel[] levels = Resources.FindObjectsOfTypeAll<SelectableLevel>();

                foreach (SelectableLevel level in levels) {
                    List<List<SpawnableEnemyWithRarity>> enemyCollections = [
                        level.Enemies,
                        level.OutsideEnemies,
                        level.DaytimeEnemies
                    ];

                    foreach (List<SpawnableEnemyWithRarity> collection in enemyCollections) {
                        foreach (SpawnableEnemyWithRarity enemy in collection) {
                            if (enemy.enemyType.enemyName.Contains("Docile Locust Bees", StringComparison.OrdinalIgnoreCase)) continue;
                            if (enemy.enemyType.enemyName.Contains("Manticoil", StringComparison.OrdinalIgnoreCase)) continue;
                            _ = uniqueEnemies.Add((enemy.enemyType.enemyName, enemy.enemyType.enemyPrefab));
                        }
                    }
                }

                foreach ((string Name, GameObject Prefab) in uniqueEnemies) {
                    _ = _AllSpawnableEnemies.TryAdd(Name, Prefab);
                }
            }

            return _AllSpawnableEnemies;
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
}
