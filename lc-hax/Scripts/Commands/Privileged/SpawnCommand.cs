using GameNetcodeStuff;
using Hax;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[PrivilegedCommand("/spawn")]
public class SpawnCommand : ICommand {

    private void SpawnEnemyOnPlayer(PlayerControllerB player, GameObject prefab, ulong amount = 1) {
        if (Helper.RoundManager == null) return;
        if (player == null) return;

        for (ulong i = 0; i < amount; i++) {
            GameObject enemy = UnityEngine.Object.Instantiate(prefab, player.transform.position, Quaternion.Euler(Vector3.zero));
            if (enemy.TryGetComponent(out NetworkObject networkObject)) {
                networkObject.Spawn(true);
                if (enemy.TryGetComponent(out EnemyAI ai)) {
                    _ = Helper.Enemies.Add(ai);
                }
            }
            else {
                UnityObject.Destroy(enemy);
            }
        }
    }

    public void Execute(StringArray args) {
        if (Helper.RoundManager == null || Helper.RoundManager.currentLevel == null) {
            Chat.Print("No round or level found.");
            return;
        }

        if (args.Length < 2) {
            Chat.Print("Usage: /spawn <player> <enemy> <amount?>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print($"Target player '{args[0]}' is not alive or found!");
            return;
        }

        string enemyNamePart = args[1];
        KeyValuePair<string, GameObject> enemyEntry = Helper.AllSpawnableEnemies
            .First(e => e.Key.Contains(enemyNamePart, StringComparison.OrdinalIgnoreCase));

        if (enemyEntry.Value == null) {
            Chat.Print($"Enemy '{enemyNamePart}' not found.");
            return;
        }

        // Parse amount; default to 1 if not provided or invalid
        ulong amount = 1;
        if (args.Length > 2 && !ulong.TryParse(args[2], out amount)) {
            Chat.Print("Invalid amount specified. Defaulting to 1.");
        }

        Chat.Print($"Spawning {amount} of {enemyEntry.Key} for {targetPlayer.playerUsername}.");
        this.SpawnEnemyOnPlayer(targetPlayer, enemyEntry.Value, amount);
    }
}
