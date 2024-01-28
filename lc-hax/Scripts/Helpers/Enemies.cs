using Mono.Cecil;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static HashSet<EnemyAI> Enemies { get; } = [];

    public static T? GetEnemy<T>() where T : EnemyAI =>
        Helper.Enemies.First(enemy => enemy is T) is T enemy ? enemy : null;


    public static Dictionary<string, GameObject> SpawnableEnemies {
        get {

            if(Helper.RoundManager == null) return [];
            if(Helper.RoundManager.currentLevel == null) return [];
            Dictionary<string, GameObject> result = [];
            foreach(SpawnableEnemyWithRarity enemy in Helper.RoundManager.currentLevel.Enemies) {
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
