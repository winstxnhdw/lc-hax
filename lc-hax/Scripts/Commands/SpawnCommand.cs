using GameNetcodeStuff;
using Hax;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[DebugCommand("/spawn")]
public class SpawnCommand : ICommand {

    


    void SpawnEnemyOnPlayer(PlayerControllerB player, GameObject prefab, ulong amount = 1) {
        if (Helper.RoundManager == null) return;
        if(player == null) return;
        for (ulong i = 0; i < amount; i++) {
            GameObject enemy = UnityEngine.Object.Instantiate<GameObject>(prefab, player.transform.position, Quaternion.Euler(Vector3.zero));
            if(enemy  != null) {
                enemy.GetComponent<NetworkObject>().Spawn(true);
                EnemyAI AI = enemy.GetComponent<EnemyAI>();
                if(AI != null) {
                    _ = Helper.Enemies.Add(AI);
                }
            }

        }
    }

    public void Execute(StringArray args) {
        if (Helper.RoundManager == null) return;
        if(Helper.RoundManager.currentLevel == null) return;
        // Check for minimum required arguments: player and enemy name
        if (args.Length < 2) {
            Chat.Print("Usage: /spawn <player> <enemy> <amount?>");
            return;
        }

        // Get the target player
        PlayerControllerB? targetPlayer = Helper.GetActivePlayer(args[0]);
        if (targetPlayer is null) {
            Chat.Print($"Target player '{args[0]}' is not alive or found!");
            return;
        }

        string name = args[1];
        string EnemyName = "";
        GameObject prefab = null;
        foreach (KeyValuePair<string, GameObject> enemy in Helper.SpawnableEnemies) {
            if (enemy.Key.Contains(name, StringComparison.OrdinalIgnoreCase)) {
                EnemyName = enemy.Key;
                prefab = enemy.Value;
                break;
            }
        }
        if (prefab == null) {
            return;
        }

        // Parse amount; default to 1 if not provided or invalid
        ulong amount = 1;
        if (args.Length > 2 && !ulong.TryParse(args[2], out amount)) {
            Chat.Print("Invalid amount specified. Defaulting to 1.");
        }

        Chat.Print($"Spawning {amount} {EnemyName} to {targetPlayer.playerUsername}");
        this.SpawnEnemyOnPlayer(targetPlayer, prefab, amount);
    }

}
