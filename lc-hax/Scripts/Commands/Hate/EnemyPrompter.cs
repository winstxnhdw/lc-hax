using System.Collections.Generic;
using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

enum BehaviourState {
    IDLE = 0,
    CHASE = 1,
    AGGRAVATED = 2,
    UNKNOWN = 3
}

public class EnemyPrompter {
    void SetBehaviourState(EnemyAI enemy, BehaviourState behaviourState) => enemy.SwitchToBehaviourState((int)behaviourState);

    void TeleportEnemyToPlayer(EnemyAI enemy, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        if (!willTeleportEnemy) return;
        enemy.transform.position = targetPlayer.transform.position;
        enemy.SyncPositionToClients();
    }

    void HandleThumper(CrawlerAI thumper, PlayerControllerB targetPlayer) => thumper.BeginChasingPlayerServerRpc((int)targetPlayer.playerClientId);

    void HandleEyelessDog(MouthDogAI eyelessDog, PlayerControllerB targetPlayer) => eyelessDog.ReactToOtherDogHowl(targetPlayer.transform.position);

    void HandleBaboonHawk(BaboonBirdAI baboonHawk, PlayerControllerB targetPlayer) {
        Threat threat = new() {
            threatScript = targetPlayer,
            lastSeenPosition = targetPlayer.transform.position,
            threatLevel = int.MaxValue,
            type = ThreatType.Player,
            focusLevel = int.MaxValue,
            timeLastSeen = Time.time,
            distanceToThreat = 0.0f,
            distanceMovedTowardsBaboon = float.MaxValue,
            interestLevel = int.MaxValue,
            hasAttacked = true
        };

        baboonHawk.SetAggressiveModeServerRpc(1);
        _ = baboonHawk.Reflect().InvokeInternalMethod("ReactToThreat", threat);
    }

    void HandleForestGiant(ForestGiantAI forestGiant, PlayerControllerB targetPlayer) {
        this.SetBehaviourState(forestGiant, BehaviourState.CHASE);
        forestGiant.StopSearch(forestGiant.roamPlanet, false);
        forestGiant.chasingPlayer = targetPlayer;
        forestGiant.investigating = true;

        _ = forestGiant.SetDestinationToPosition(targetPlayer.transform.position);
        _ = forestGiant.Reflect().SetInternalField("lostPlayerInChase", false);
    }

    void HandleSnareFlea(CentipedeAI snareFlea, PlayerControllerB targetPlayer) {
        this.SetBehaviourState(snareFlea, BehaviourState.AGGRAVATED);
        snareFlea.ClingToPlayerServerRpc(targetPlayer.playerClientId);
    }

    void HandleBracken(FlowermanAI bracken) {
        this.SetBehaviourState(bracken, BehaviourState.AGGRAVATED);
        bracken.EnterAngerModeServerRpc(20);
    }

    void HandleBunkerSpider(SandSpiderAI bunkerSpider) {
        this.SetBehaviourState(bunkerSpider, BehaviourState.AGGRAVATED);
        bunkerSpider.meshContainer.position = bunkerSpider.transform.position;
        bunkerSpider.SyncMeshContainerPositionToClients();

        _ = bunkerSpider.Reflect()
                         .SetInternalField("onWall", false)?
                         .SetInternalField("watchFromDistance", false);
    }

    void HandleBee(RedLocustBees bee, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(bee, targetPlayer, willTeleportEnemy);
        this.SetBehaviourState(bee, BehaviourState.AGGRAVATED);
        bee.hive.isHeld = true;
    }

    void HandleHoardingBug(HoarderBugAI hoardingBug, PlayerControllerB targetPlayer) {
        this.SetBehaviourState(hoardingBug, BehaviourState.AGGRAVATED);
        hoardingBug.angryAtPlayer = targetPlayer;
        hoardingBug.angryTimer = float.MaxValue;

        _ = hoardingBug.Reflect()
                       .SetInternalField("lostPlayerInChase", false)?
                       .InvokeInternalMethod("SyncNestPositionServerRpc", targetPlayer.transform.position);
    }

    void HandleNutcracker(NutcrackerEnemyAI nutcracker, PlayerControllerB targetPlayer) {
        this.SetBehaviourState(nutcracker, BehaviourState.AGGRAVATED);
        nutcracker.SeeMovingThreatServerRpc((int)targetPlayer.playerClientId);

        _ = nutcracker.Reflect()
                      .SetInternalField("lastSeenPlayerPos", targetPlayer.transform.position)?
                      .SetInternalField("timeSinceSeeingTarget", 0);
    }

    void HandleMaskedPlayer(MaskedPlayerEnemy maskedPlayer, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(maskedPlayer, targetPlayer, willTeleportEnemy);
        this.SetBehaviourState(maskedPlayer, BehaviourState.CHASE);
    }

    void HandleCoilHead(SpringManAI coilHead, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(coilHead, targetPlayer, willTeleportEnemy);
        this.SetBehaviourState(coilHead, BehaviourState.CHASE);
    }

    void HandleEnemy(EnemyAI enemy, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        switch (enemy) {
            case CrawlerAI thumper:
                this.HandleThumper(thumper, targetPlayer);
                break;

            case MouthDogAI eyelessDog:
                this.HandleEyelessDog(eyelessDog, targetPlayer);
                break;

            case BaboonBirdAI baboonHawk:
                this.HandleBaboonHawk(baboonHawk, targetPlayer);
                break;

            case ForestGiantAI forestGiant:
                this.HandleForestGiant(forestGiant, targetPlayer);
                break;

            case CentipedeAI snareFlea:
                this.HandleSnareFlea(snareFlea, targetPlayer);
                break;

            case FlowermanAI bracken:
                this.HandleBracken(bracken);
                break;

            case SandSpiderAI bunkerSpider:
                this.HandleBunkerSpider(bunkerSpider);
                break;

            case RedLocustBees bee:
                this.HandleBee(bee, targetPlayer, willTeleportEnemy);
                break;

            case HoarderBugAI hoardingBug:
                this.HandleHoardingBug(hoardingBug, targetPlayer);
                break;

            case NutcrackerEnemyAI nutcracker:
                this.HandleNutcracker(nutcracker, targetPlayer);
                break;

            case MaskedPlayerEnemy maskedPlayer:
                this.HandleMaskedPlayer(maskedPlayer, targetPlayer, willTeleportEnemy);
                break;

            case SpringManAI:
                this.HandleCoilHead((SpringManAI)enemy, targetPlayer, willTeleportEnemy);
                break;

            case PufferAI:
            case JesterAI:
                this.SetBehaviourState(enemy, BehaviourState.AGGRAVATED);
                break;

            case SandWormAI:
                this.SetBehaviourState(enemy, BehaviourState.CHASE);
                break;

            default:
                this.SetBehaviourState(enemy, BehaviourState.CHASE);
                break;
        }
    }

    public List<string> PromptEnemiesToTarget(
        PlayerControllerB player,
        bool funnyRevive = false,
        bool willTeleportEnemies = false
    ) {
        List<string> enemyNames = [];

        if (!Helper.RoundManager.IsNotNull(out RoundManager roundManager)) {
            return enemyNames;
        }

        if (funnyRevive) {
            Console.Print("Funny revive!");
        }

        _ = roundManager.Reflect().InvokeInternalMethod("RefreshEnemiesList");

        roundManager.SpawnedEnemies.ForEach((enemy) => {
            if (enemy is DocileLocustBeesAI or DoublewingAI or BlobAI or DressGirlAI or LassoManAI) return;

            if (funnyRevive) {
                enemy.isEnemyDead = false;
                enemy.enemyHP = enemy.enemyHP <= 0 ? 1 : enemy.enemyHP;
            }

            enemy.serverPosition = Vector3.positiveInfinity;
            enemy.targetPlayer = player;
            enemy.ChangeEnemyOwnerServerRpc(roundManager.playersManager.localPlayerController.actualClientId);
            enemy.SetMovingTowardsTargetPlayer(player);
            enemyNames.Add(enemy.enemyType.enemyName);
            this.HandleEnemy(enemy, player, willTeleportEnemies);
        });

        return enemyNames;
    }
}
