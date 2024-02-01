using GameNetcodeStuff;
using System.Collections.Generic;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static HashSet<EnemyAI> Enemies { get; } = [];

    public static T? GetEnemy<T>() where T : EnemyAI =>
        Helper.Enemies.First(enemy => enemy is T) is T enemy ? enemy : null;


    public static PlayerControllerB? GetPlayerAboutToKilledByEnemy(int playerObjectID) {
        PlayerControllerB[] players = Helper.Players;
        return players.First(player => (int)player.playerClientId == playerObjectID);
    }

    public static bool IsEnemyAboutToKillLocalPlayer(int PlayerID) {
        PlayerControllerB? player = Helper.GetPlayerAboutToKilledByEnemy(PlayerID);
        return player != null && player.isSelf();
    }

    public static bool IsEnemyAboutToKillLocalPlayer(this EnemyAI instance, Collider other) {
        if (instance == null) return false;
        if (other == null) return false;
        PlayerControllerB playerControllerB = instance.MeetsStandardPlayerCollisionConditions(other, false, false);
        return playerControllerB != null && playerControllerB.isSelf();
    }


    public static Dictionary<string, GameObject> SpawnableEnemies {
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

}
