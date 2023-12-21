using System.Collections.Generic;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public static partial class Helper {
    static void TeleportEnemyToPlayer(EnemyAI enemy, PlayerControllerB player) {
        enemy.transform.position = player.transform.position - player.transform.forward;
        enemy.SyncPositionToClients();
    }

    public static List<string> PromptEnemiesToTarget(PlayerControllerB player, bool funnyRevive) {
        List<string> enemyNames = [];

        if (!Helper.RoundManager.IsNotNull(out RoundManager roundManager)) {
            return enemyNames;
        }

        _ = roundManager.Reflect().InvokeInternalMethod("RefreshEnemiesList");

        if (funnyRevive) {
            Console.Print("Funny revive!");
        }

        roundManager.SpawnedEnemies.ForEach((enemy) => {
            if (enemy is DocileLocustBeesAI or DoublewingAI or BlobAI or DressGirlAI or LassoManAI) return;

            if (funnyRevive) {
                enemy.isEnemyDead = false;
                enemy.enemyHP = enemy.enemyHP <= 0 ? 1 : enemy.enemyHP;
            }

            enemy.serverPosition = Vector3.positiveInfinity;
            enemy.targetPlayer = player;
            enemyNames.Add(enemy.enemyType.enemyName);
            enemy.ChangeEnemyOwnerServerRpc(roundManager.playersManager.localPlayerController.actualClientId);

            if (enemy is CrawlerAI thumper) {
                thumper.BeginChasingPlayerServerRpc((int)player.playerClientId);
            }

            else if (enemy is MouthDogAI eyelessDog) {
                eyelessDog.ReactToOtherDogHowl(player.transform.position);
            }

            else if (enemy is BaboonBirdAI baboonHawk) {
                Threat threat = new() {
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

                _ = baboonHawk.Reflect().InvokeInternalMethod("ReactToThreat", threat);
            }

            else if (enemy is ForestGiantAI giant) {
                giant.chasingPlayer = player;
                giant.timeSpentStaring = 10;
                giant.SwitchToBehaviourState(1);

                _ = giant.Reflect().SetInternalField("lostPlayerInChase", false);

            }

            else if (enemy is SandWormAI) {
                if (!player.isInsideFactory) TeleportEnemyToPlayer(enemy, player);
                enemy.SwitchToBehaviourState(1);
            }

            else if (enemy is MaskedPlayerEnemy) {
                TeleportEnemyToPlayer(enemy, player);
                enemy.SwitchToBehaviourState(1);
            }

            else if (enemy is SpringManAI) {
                if (player.isInsideFactory) TeleportEnemyToPlayer(enemy, player);
                enemy.SwitchToBehaviourState(1);
            }

            else if (enemy is PufferAI or JesterAI) {
                enemy.SwitchToBehaviourState(2);
            }

            else if (enemy is CentipedeAI snareFlea) {
                if (player.isInsideFactory) TeleportEnemyToPlayer(enemy, player);
                enemy.SwitchToBehaviourState(2);
            }

            else if (enemy is FlowermanAI bracken) {
                if (player.isInsideFactory) TeleportEnemyToPlayer(enemy, player);
                bracken.SwitchToBehaviourState(2);
                bracken.EnterAngerModeServerRpc(20);
            }

            else if (enemy is SandSpiderAI spider) {
                spider.meshContainer.position = player.transform.position;
                spider.SwitchToBehaviourState(2);
                spider.SyncMeshContainerPositionToClients();

                _ = spider.Reflect()
                          .SetInternalField("onWall", false)?
                          .SetInternalField("watchFromDistance", false);
            }

            else if (enemy is HoarderBugAI hoardingBug) {
                hoardingBug.angryAtPlayer = player;
                hoardingBug.angryTimer = 1000;
                hoardingBug.SwitchToBehaviourState(2);

                _ = hoardingBug.Reflect()
                               .SetInternalField("lostPlayerInChase", false)?
                               .InvokeInternalMethod("SyncNestPositionServerRpc", player.transform.position);
            }

            else if (enemy is RedLocustBees bees) {
                if (!player.isInsideFactory) TeleportEnemyToPlayer(enemy, player);

                bees.SwitchToBehaviourState(2);
                bees.hive.isHeld = true;
            }

            else if (enemy is NutcrackerEnemyAI nutcracker) {
                nutcracker.SwitchToBehaviourState(2);
                nutcracker.SeeMovingThreatServerRpc((int)player.playerClientId);
                _ = nutcracker.Reflect()
                              .SetInternalField("lastSeenPlayerPos", player.transform.position)?
                              .SetInternalField("timeSinceSeeingTarget", 0);
            }
        });

        return enemyNames;
    }
}
