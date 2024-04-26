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

    /// <summary>
    /// Only takes ownership of the enemy if it is not already owned.
    /// </summary>
    /// <param name="enemy"></param>
    internal static void TakeOwnerShipIfNotOwned(this EnemyAI enemy) {
        if (enemy.IsOwner()) return;
        enemy.TakeOwnership();
    }

    /// <summary>
    /// Checks if the specified player is the owner of the enemy.
    /// </summary>
    internal static bool IsOwner(this EnemyAI enemy, PlayerControllerB player) {
        if (player is null) return false;
        int currentOwnershipOnThisClient = enemy.currentOwnershipOnThisClient;
        ulong networkOwner = enemy.thisNetworkObject.OwnerClientId;
        ulong playerID = player.IsSelf() ? player.actualClientId : (ulong)player.PlayerIndex();
        return currentOwnershipOnThisClient == (int)playerID && networkOwner == playerID;
    }

    /// <summary>
    /// Checks if the local player is the owner of the enemy.
    /// </summary>
    internal static bool IsOwner(this EnemyAI enemy) => Helper.LocalPlayer is PlayerControllerB localPlayer && enemy.IsOwner(localPlayer);

    /// <summary>
    /// Gives the local player ownership of the enemy.
    /// </summary>
    internal static void TakeOwnership(this EnemyAI enemy) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        enemy.SetOwner(localPlayer);
    }
    /// <summary>
    /// Restores the enemy Ownership to the Host.
    /// </summary>
    internal static void RemoveOwnership(this EnemyAI enemy) => enemy.SetOwner(Helper.Players[0]);

    /// <summary>
    /// Sets the owner of the enemy to the specified player.
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="player"></param>
    internal static void SetOwner(this EnemyAI enemy, PlayerControllerB? player) {
        if (player is null) return;
        if (player.IsSelf()) {
            enemy.SetOwner(player.actualClientId);
        }
        else {
            enemy.SetOwner((ulong)player.PlayerIndex());
        }
    }

    /// <summary>
    /// Sets the owner of the enemy to the specified client with RPC.
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="clientID"></param>
    internal static void SetOwner(this EnemyAI enemy, ulong clientID) {
        enemy.ChangeOwnershipOfEnemy(clientID);
        enemy.ChangeEnemyOwnerServerRpc(clientID);
    }
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

    internal static void Kill(this EnemyAI enemy) {
        enemy.TakeOwnership();

        switch (enemy) {
            case NutcrackerEnemyAI nutcracker:
                nutcracker.KillEnemy();
                break;
            case ButlerEnemyAI butler:
                butler.KillEnemy();
                break;

            default:
                enemy.KillEnemyServerRpc(true);
                break;
        }
    }
    /// <summary>
    /// Updates enemy navmesh and resets the searchs routines
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="isOutside"></param>
    /// <param name="controller">If None given, it will use PossessionMod's and get a suitable controller to reset it's searches</param>
    internal static void SetOutsideStatus(this EnemyAI enemy, bool isOutside, IController? controller = null) {
        if (enemy.isOutside == isOutside) return;

        enemy.isOutside = isOutside;
        enemy.allAINodes = GameObject.FindGameObjectsWithTag(enemy.isOutside ? "OutsideAINode" : "AINode");
        _ = enemy.agent.Warp(enemy.transform.position);
        enemy.FinishedCurrentSearchRoutine();
        enemy.StopSearch(enemy.currentSearch, true);
        enemy.SyncPositionToClients();
        enemy.agent.ResetPath();
        Transform closestNodePos = enemy.ChooseClosestNodeToPosition(enemy.transform.position, false, 0);
        _ = enemy.SetDestinationToPosition(closestNodePos.position, true);
        enemy.EnableEnemyMesh(true, false);
        if (controller is null) {
            if (PossessionMod.Instance is not PossessionMod possession) return;
            controller = possession.GetEnemyController(enemy);
        }
        controller?.OnOutsideStatusChange(enemy);
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
        _ = Enemies.Add(enemyAI);
        return enemyAI;
    }

    /// <summary>
    /// Finds the closest player to the enemy within a specified radius, prioritizing players on the ground.
    /// If no players are found, it defaults to the Host.
    /// </summary>
    /// <param name="enemy">The enemy AI from which to search for nearby players.</param>
    /// <param name="radius">The search radius. Default is 1.5f, but will be scaled by a factor of 3.</param>
    /// <returns>The closest PlayerControllerB instance or Helper.Players[0] if no player is found.</returns>
    internal static PlayerControllerB FindClosestPlayer(this EnemyAI enemy, float radius = 1.5f) {
        radius *= 3;

        float groundLevel = enemy.transform.position.y;

        Collider[] playersInRange = Physics.OverlapSphere(enemy.transform.position, radius, LayerMask.GetMask("Player"));

        PlayerControllerB closestPlayer = null;
        float closestDistanceSqr = Mathf.Infinity;
        float groundPriorityWeight = 0.1f; // Adjust this weight to prioritize ground distance more or less
        foreach (Collider playerCollider in playersInRange) {
            PlayerControllerB player = playerCollider.GetComponent<PlayerControllerB>();
            if (player) {
                Vector3 playerPosition = player.transform.position;
                float verticalOffset = Mathf.Abs(playerPosition.y - groundLevel); // Distance from the ground
                float distanceSqr = (playerPosition - enemy.transform.position).sqrMagnitude + (verticalOffset * groundPriorityWeight);

                if (distanceSqr < closestDistanceSqr) {
                    closestDistanceSqr = distanceSqr;
                    closestPlayer = player;
                }
            }
        }
        Console.WriteLine($"Enemy: {enemy.enemyType.enemyName} | Closest Player: {(closestPlayer ?? Helper.Players[0])?.playerUsername}");
        return closestPlayer ?? Helper.Players[0];
    }
    /// <summary>
    /// Converts the Player to a Threat object with max priority for any enemy.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    internal static Threat ToThreat(this PlayerControllerB player) {
        return new() {
            threatScript = player,
            lastSeenPosition = player.transform.position,
            threatLevel = int.MaxValue,
            type = ThreatType.Player,
            focusLevel = int.MaxValue,
            timeLastSeen = Time.time,
            distanceToThreat = 0.0f,
            distanceMovedTowardsBaboon = float.MaxValue,
            interestLevel = int.MaxValue,
            hasAttacked = true

        };
    }

}
