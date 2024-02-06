using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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
        return player != null && player.isSelf();
    }

    internal static bool IsLocalPlayerAboutToGetKilledByEnemy(this EnemyAI instance, Collider other) {
        if (instance == null) return false;
        if (other == null) return false;
        PlayerControllerB playerControllerB = instance.MeetsStandardPlayerCollisionConditions(other, false, false);
        return playerControllerB != null && playerControllerB.isSelf();
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
        Collider[] Search = Physics.OverlapSphere(instance.transform.position, grabRange);
        for (int i = 0; i < Search.Length; i++) {
            if (Search[i].TryGetComponent(out GrabbableObject item))
                return item;
        }

        return null;
    }

    public static void MouthDogChasePlayer(this MouthDogAI instance, PlayerControllerB player) {
        if (instance == null) return;
        if (player == null) return;
        if (instance.currentBehaviourStateIndex is 0 or 1) {
           _ = instance.Reflect().InvokeInternalMethod("ChaseLocalPlayer");
        }
        else {
            if (instance.currentBehaviourStateIndex != 2 || instance.Reflect().GetInternalField<bool>("inLunge"))
                return;
            instance.transform.LookAt(player.transform.position);
            instance.transform.localEulerAngles = new Vector3(0.0f, player.transform.eulerAngles.y, 0.0f);
            _ = instance.Reflect().SetInternalField("inLunge", true);
            _ = instance.Reflect().InvokeInternalMethod("EnterLunge");
        }
    }
}
