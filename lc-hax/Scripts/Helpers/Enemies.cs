using System;
using System.Collections.Generic;
using System.Linq;

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
                foreach (EnemyType enemy in Resources.FindObjectsOfTypeAll<EnemyType>()) {
                    if (enemy.enemyName.Contains("Docile Locust Bees", StringComparison.OrdinalIgnoreCase)) continue;
                    if (enemy.enemyName.Contains("Manticoil", StringComparison.OrdinalIgnoreCase)) continue;
                    _ = _AllSpawnableEnemies.TryAdd(enemy.enemyName, enemy.enemyPrefab);
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
