#region

using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Hax;

static partial class Helper {
    /// <summary>
    ///     Gets a set of active enemies if the game is not in the ship phase.
    /// </summary>
    internal static HashSet<EnemyAI> Enemies { get; } = StartOfRound is { inShipPhase: true }
        ? new HashSet<EnemyAI>()
        : FindObjects<EnemyAI>()
            .WhereIsNotNull()
            .Where(enemy => enemy.IsSpawned)
            .ToHashSet();

    /// <summary>
    ///     Gets a dictionary of hostile enemy types and their corresponding prefabs.
    /// </summary>
    internal static Dictionary<string, GameObject> HostileEnemies { get; } =
        Resources.FindObjectsOfTypeAll<EnemyType>()
            .Where(IsHostileEnemy)
            .GroupBy(enemy => enemy.enemyName)
            .ToDictionary(enemyGroup => enemyGroup.Key, enemy => Enumerable.First(enemy).enemyPrefab);

    /// <summary>
    ///     Takes ownership of the enemy if it is not already owned.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    internal static void TakeOwnerShipIfNotOwned(this EnemyAI enemy) {
        if (enemy.IsOwner()) return;
        enemy.TakeOwnership();
    }

    /// <summary>
    ///     Checks if the specified player is the owner of the enemy.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    /// <param name="player">The player controller.</param>
    /// <returns><c>true</c> if the player is the owner; otherwise, <c>false</c>.</returns>
    internal static bool IsOwner(this EnemyAI enemy, PlayerControllerB player) {
        if (player is null) return false;
        int currentOwnershipOnThisClient = enemy.currentOwnershipOnThisClient;
        ulong networkOwner = enemy.thisNetworkObject.OwnerClientId;
        ulong playerID = player.IsSelf() ? player.actualClientId : (ulong)player.GetPlayerId();
        return currentOwnershipOnThisClient == (int)playerID && networkOwner == playerID;
    }

    /// <summary>
    ///     Checks if the local player is the owner of the enemy.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    /// <returns><c>true</c> if the local player is the owner; otherwise, <c>false</c>.</returns>
    internal static bool IsOwner(this EnemyAI enemy) =>
        LocalPlayer is PlayerControllerB localPlayer && enemy.IsOwner(localPlayer);

    /// <summary>
    ///     Gives the local player ownership of the enemy.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    internal static void TakeOwnership(this EnemyAI enemy) {
        if (LocalPlayer is not PlayerControllerB localPlayer) return;
        enemy.SetOwner(localPlayer);
    }

    /// <summary>
    ///     Restores the enemy ownership to the host.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    internal static void RemoveOwnership(this EnemyAI enemy) => enemy.SetOwner(Players[0]);

    /// <summary>
    ///     Sets the owner of the enemy to the specified player.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    /// <param name="player">The player controller.</param>
    internal static void SetOwner(this EnemyAI enemy, PlayerControllerB? player) {
        if (player is null) return;
        enemy.SetOwner((ulong)player.GetPlayerId());
    }

    /// <summary>
    ///     Sets the owner of the enemy to the specified client with RPC.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    /// <param name="clientID">The client ID.</param>
    internal static void SetOwner(this EnemyAI enemy, ulong clientID) {
        enemy.ChangeOwnershipOfEnemy(clientID);
        enemy.ChangeEnemyOwnerServerRpc(clientID);
    }

    /// <summary>
    ///     Determines if the enemy type is hostile.
    /// </summary>
    /// <param name="enemy">The enemy type.</param>
    /// <returns><c>true</c> if the enemy is hostile; otherwise, <c>false</c>.</returns>
    internal static bool IsHostileEnemy(EnemyType enemy) =>
        !enemy.enemyName.Contains("Docile Locust Bees", StringComparison.InvariantCultureIgnoreCase) &&
        !enemy.enemyName.Contains("Manticoil", StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    ///     Gets an enemy of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the enemy.</typeparam>
    /// <returns>An enemy of the specified type.</returns>
    internal static T? GetEnemy<T>() where T : EnemyAI => Enemies.First(enemy => enemy is T) is T enemy ? enemy : null;

    /// <summary>
    ///     Kills the specified enemy.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    internal static void Kill(this EnemyAI enemy) {
        enemy.TakeOwnerShipIfNotOwned();
        bool Destroy = true;
        if (!enemy.isEnemyDead) {
            switch (enemy) {
                case NutcrackerEnemyAI nutcracker:
                    nutcracker.KillEnemy();
                    Destroy = false;
                    break;
                case ButlerEnemyAI butler:
                    butler.KillEnemy();
                    Destroy = false;
                    break;
                case FlowermanAI flowerman:
                    flowerman.KillEnemy();
                    break;
                default:
                    break;
            }

            enemy.HitEnemy(int.MaxValue, null, false, -1);
            enemy.KillEnemyServerRpc(Destroy);
        }

        if (LocalPlayer is not PlayerControllerB localPlayer) return;
        if (!Destroy) return;
        if (localPlayer.IsHost)
            if (enemy.TryGetComponent(out NetworkObject networkObject))
                networkObject.Despawn(true);
    }

    /// <summary>
    ///     Updates enemy navmesh and resets the search routines.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    /// <param name="isOutside">If set to <c>true</c>, sets the enemy to outside status.</param>
    /// <param name="controller">
    ///     The controller. If none is given, it will use PossessionMod's and get a suitable controller to
    ///     reset its searches.
    /// </param>
    internal static void SetOutsideStatus(this EnemyAI enemy, bool isOutside, IController? controller = null) {

        enemy.isOutside = isOutside;
        enemy.allAINodes = GameObject.FindGameObjectsWithTag(enemy.isOutside ? "OutsideAINode" : "AINode");
        _ = enemy.agent.Warp(enemy.transform.position);
        enemy.FinishedCurrentSearchRoutine();
        enemy.StopSearch(enemy.currentSearch, true);
        enemy.SyncPositionToClients();
        enemy.agent.ResetPath();
        Transform? closestNodePos = enemy.ChooseClosestNodeToPosition(enemy.transform.position, false, 0);
        _ = enemy.SetDestinationToPosition(closestNodePos.position, true);
        enemy.EnableEnemyMesh(true, false);
        if (controller is null) {
            if (PossessionMod.Instance is not PossessionMod possession) return;
            controller = possession.GetEnemyController(enemy);
        }

        controller?.OnOutsideStatusChange(enemy);
    }

    /// <summary>
    ///     Checks if the enemy is in the specified behaviour state.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    /// <param name="state">The behaviour state.</param>
    /// <returns><c>true</c> if the enemy is in the specified state; otherwise, <c>false</c>.</returns>
    internal static bool IsBehaviourState(this EnemyAI enemy, Enum state) =>
        enemy.currentBehaviourStateIndex == Convert.ToInt32(state);

    /// <summary>
    ///     Sets the behaviour state of the enemy.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    /// <param name="state">The behaviour state.</param>
    internal static void SetBehaviourState(this EnemyAI enemy, Enum state) {
        if (enemy.IsBehaviourState(state)) return;
        enemy.SwitchToBehaviourServerRpc(Convert.ToInt32(state));
    }

    /// <summary>
    ///     Finds a nearby item for the enemy to grab.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    /// <param name="grabRange">The grab range.</param>
    /// <returns>The nearby grabbable object.</returns>
    internal static GrabbableObject? FindNearbyItem(this EnemyAI enemy, float grabRange = 1.0f) {
        foreach (Collider? collider in Physics.OverlapSphere(enemy.transform.position, grabRange)) {
            if (!collider.TryGetComponent(out GrabbableObject item)) continue;
            if (!item.TryGetComponent(out NetworkObject _)) continue;

            return item;
        }

        return null;
    }

    /// <summary>
    ///     Spawns a specified amount of enemies at the given position.
    /// </summary>
    /// <param name="position">The spawn position.</param>
    /// <param name="prefab">The enemy prefab.</param>
    /// <param name="amount">The amount of enemies to spawn.</param>
    internal static void SpawnEnemies(Vector3 position, GameObject prefab, ulong amount = 1) {
        for (ulong i = 0; i < amount; i++) _ = SpawnEnemy(position, prefab);
    }

    /// <summary>
    ///     Spawns an enemy at the specified position.
    /// </summary>
    /// <param name="position">The spawn position.</param>
    /// <param name="prefab">The enemy prefab.</param>
    /// <returns>The spawned enemy AI.</returns>
    internal static EnemyAI? SpawnEnemy(Vector3 position, GameObject prefab) {
        if (prefab == null) return null;
        GameObject? enemy = Object.Instantiate(prefab, position, Quaternion.identity);

        if (!enemy.TryGetComponent(out NetworkObject networkObject)) {
            Object.Destroy(enemy);
            return null;
        }

        networkObject.Spawn(true);
        if (!enemy.TryGetComponent(out EnemyAI enemyAI)) {
            Object.Destroy(enemy);
            return null;
        }

        _ = Enemies.Add(enemyAI);
        return enemyAI;
    }

    /// <summary>
    ///     Finds the closest player to the enemy within a specified radius, prioritizing players on the ground. Defaults to
    ///     the Host if no player is found.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    /// <param name="radius">The search radius. Default is 1.5f, but will be scaled by a factor of 3.</param>
    /// <returns>The closest PlayerControllerB instance or Host if no player is found.</returns>
    internal static PlayerControllerB FindClosestPlayer(this EnemyAI enemy, float radius = 1.5f) {
        radius *= 3;

        float groundLevel = enemy.transform.position.y;

        Collider[] playersInRange =
            Physics.OverlapSphere(enemy.transform.position, radius, LayerMask.GetMask("Player"));

        PlayerControllerB closestPlayer = null;
        float closestDistanceSqr = Mathf.Infinity;
        float groundPriorityWeight = 0.1f; // Adjust this weight to prioritize ground distance more or less
        foreach (Collider playerCollider in playersInRange) {
            PlayerControllerB? player = playerCollider.GetComponent<PlayerControllerB>();
            if (player) {
                Vector3 playerPosition = player.transform.position;
                float verticalOffset = Mathf.Abs(playerPosition.y - groundLevel); // Distance from the ground
                float distanceSqr = (playerPosition - enemy.transform.position).sqrMagnitude +
                                    verticalOffset * groundPriorityWeight;

                if (distanceSqr < closestDistanceSqr) {
                    closestDistanceSqr = distanceSqr;
                    closestPlayer = player;
                }
            }
        }

        Console.WriteLine(
            $"Enemy: {enemy.enemyType.enemyName} | Closest Player: {(closestPlayer ?? Players[0])?.playerUsername}");
        return closestPlayer ?? Players[0];
    }

    /// <summary>
    ///     Converts the Player to a Threat object with max priority for any enemy.
    /// </summary>
    /// <param name="player">The player controller.</param>
    /// <returns>The Threat object representing the player.</returns>
    internal static Threat ToThreat(this PlayerControllerB player) =>
        new() {
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

    /// <summary>
    ///     Determines if the enemy can die.
    /// </summary>
    /// <param name="enemy">The enemy AI.</param>
    /// <returns><c>true</c> if the enemy can die; otherwise, <c>false</c>.</returns>
    internal static bool CanEnemyDie(this EnemyAI enemy) {
        if (enemy == null) return false;
        switch (enemy) {
            case RadMechAI _:
            case BlobAI _:
            case PufferAI _:
            case DoublewingAI _:
            case RedLocustBees _:
            case ButlerBeesEnemyAI _:
            case JesterAI _:
            case SandWormAI _:
            case SpringManAI _:
            case TestEnemy _:
            case LassoManAI _:
            case DocileLocustBeesAI _:
            case DressGirlAI _:
                return false;

            default:
                return enemy.enemyType.canDie;
        }
    }
}
