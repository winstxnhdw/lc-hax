using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

enum BehaviourState {
    IDLE = 0,
    CHASE = 1,
    AGGRAVATED = 2,
    UNKNOWN = 3
}

sealed class EnemyPromptHandler {
    static void TeleportEnemyToPlayer(
        EnemyAI enemy,
        PlayerControllerB targetPlayer,
        bool willTeleportEnemy,
        bool allowedOutside = false,
        bool allowedInside = false
    ) {
        if (!willTeleportEnemy) return;
        if (!allowedOutside && !targetPlayer.isInsideFactory) return;
        if (!allowedInside && targetPlayer.isInsideFactory) return;

        enemy.transform.position = targetPlayer.transform.position;
        enemy.SyncPositionToClients();
    }

    static void HandleThumper(CrawlerAI thumper, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(thumper, targetPlayer, willTeleportEnemy, allowedInside: true);
        thumper.BeginChasingPlayerServerRpc(targetPlayer.PlayerIndex());
    }

    static void HandleEyelessDog(MouthDogAI eyelessDog, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(eyelessDog, targetPlayer, willTeleportEnemy, true);
        eyelessDog.ReactToOtherDogHowl(targetPlayer.transform.position);
    }

    static void HandleBaboonHawk(BaboonBirdAI baboonHawk, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(baboonHawk, targetPlayer, willTeleportEnemy, true);

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
        baboonHawk.ReactToThreat(threat);
    }

    static void HandleForestGiant(ForestGiantAI forestGiant, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(forestGiant, targetPlayer, willTeleportEnemy, true);
        forestGiant.SetBehaviourState(BehaviourState.CHASE);
        forestGiant.StopSearch(forestGiant.roamPlanet, false);
        forestGiant.chasingPlayer = targetPlayer;
        forestGiant.investigating = true;

        _ = forestGiant.SetDestinationToPosition(targetPlayer.transform.position);
        forestGiant.lostPlayerInChase = false;
    }

    static void HandleSnareFlea(CentipedeAI snareFlea, PlayerControllerB targetPlayer) {
        if (!targetPlayer.isInsideFactory) return;
        snareFlea.SetBehaviourState(BehaviourState.CHASE);
    }

    static void HandleBracken(FlowermanAI bracken, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(bracken, targetPlayer, willTeleportEnemy, allowedInside: true);
        bracken.SetBehaviourState(BehaviourState.AGGRAVATED);
        bracken.EnterAngerModeServerRpc(float.MaxValue);
    }

    static void HandleBunkerSpider(SandSpiderAI bunkerSpider, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(bunkerSpider, targetPlayer, willTeleportEnemy, allowedInside: true);
        bunkerSpider.SetBehaviourState(BehaviourState.AGGRAVATED);

        Vector3 playerPosition = targetPlayer.transform.position;

        bunkerSpider.SpawnWebTrapServerRpc(
            playerPosition,
            playerPosition + (targetPlayer.transform.forward * 5.0f)
        );

        bunkerSpider.watchFromDistance = false;
        bunkerSpider.chaseTimer = float.MaxValue;
    }

    static void HandleBee(RedLocustBees bee, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(bee, targetPlayer, willTeleportEnemy, true);
        bee.SetBehaviourState(BehaviourState.AGGRAVATED);
    }

    static void HandleHoardingBug(HoarderBugAI hoardingBug, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(hoardingBug, targetPlayer, willTeleportEnemy, allowedInside: true);
        hoardingBug.SetBehaviourState(BehaviourState.AGGRAVATED);
        hoardingBug.angryAtPlayer = targetPlayer;
        hoardingBug.angryTimer = float.MaxValue;
        hoardingBug.lostPlayerInChase = false;
        hoardingBug.SyncNestPositionServerRpc(targetPlayer.transform.position);
    }

    static void HandleNutcracker(NutcrackerEnemyAI nutcracker, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(nutcracker, targetPlayer, willTeleportEnemy, true, true);

        nutcracker.StopInspection();
        nutcracker.SeeMovingThreatServerRpc(targetPlayer.PlayerIndex());
        nutcracker.AimGunServerRpc(targetPlayer.transform.position);
        nutcracker.lastSeenPlayerPos = targetPlayer.transform.position;
        nutcracker.timeSinceSeeingTarget = 0f;
    }

    static void HandleMaskedPlayer(MaskedPlayerEnemy maskedPlayer, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(maskedPlayer, targetPlayer, willTeleportEnemy, true, true);
        maskedPlayer.SetBehaviourState(BehaviourState.CHASE);
        maskedPlayer.SetEnemyOutside(!targetPlayer.isInsideFactory);
    }

    static void HandleCoilHead(SpringManAI coilHead, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(coilHead, targetPlayer, willTeleportEnemy, allowedInside: true);
        coilHead.SetBehaviourState(BehaviourState.CHASE);
        coilHead.SetAnimationGoServerRpc();
        coilHead.creatureAnimator.SetFloat("walkSpeed", 5.0f);
        coilHead.mainCollider.isTrigger = true;
        coilHead.agent.speed = 5.0f;
    }

    static void HandleSporeLizard(PufferAI sporeLizard, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(sporeLizard, targetPlayer, willTeleportEnemy, allowedInside: true);
        sporeLizard.SetBehaviourState(BehaviourState.AGGRAVATED);
    }

    static void HandleJester(JesterAI jester, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(jester, targetPlayer, willTeleportEnemy, allowedInside: true);
        jester.SetBehaviourState(BehaviourState.AGGRAVATED);
    }

    static void HandleEarthLeviathan(SandWormAI earthLeviathan, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        TeleportEnemyToPlayer(earthLeviathan, targetPlayer, willTeleportEnemy, true);
        earthLeviathan.SetBehaviourState(BehaviourState.CHASE);
    }

    internal static void HandleEnemy(EnemyAI enemy, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        switch (enemy) {
            case CrawlerAI thumper:
                EnemyPromptHandler.HandleThumper(thumper, targetPlayer, willTeleportEnemy);
                break;

            case MouthDogAI eyelessDog:
                EnemyPromptHandler.HandleEyelessDog(eyelessDog, targetPlayer, willTeleportEnemy);
                break;

            case BaboonBirdAI baboonHawk:
                EnemyPromptHandler.HandleBaboonHawk(baboonHawk, targetPlayer, willTeleportEnemy);
                break;

            case ForestGiantAI forestGiant:
                EnemyPromptHandler.HandleForestGiant(forestGiant, targetPlayer, willTeleportEnemy);
                break;

            case CentipedeAI snareFlea:
                EnemyPromptHandler.HandleSnareFlea(snareFlea, targetPlayer);
                break;

            case FlowermanAI bracken:
                EnemyPromptHandler.HandleBracken(bracken, targetPlayer, willTeleportEnemy);
                break;

            case SandSpiderAI bunkerSpider:
                EnemyPromptHandler.HandleBunkerSpider(bunkerSpider, targetPlayer, willTeleportEnemy);
                break;

            case RedLocustBees bee:
                EnemyPromptHandler.HandleBee(bee, targetPlayer, willTeleportEnemy);
                break;

            case HoarderBugAI hoardingBug:
                EnemyPromptHandler.HandleHoardingBug(hoardingBug, targetPlayer, willTeleportEnemy);
                break;

            case NutcrackerEnemyAI nutcracker:
                EnemyPromptHandler.HandleNutcracker(nutcracker, targetPlayer, willTeleportEnemy);
                break;

            case MaskedPlayerEnemy maskedPlayer:
                EnemyPromptHandler.HandleMaskedPlayer(maskedPlayer, targetPlayer, willTeleportEnemy);
                break;

            case SpringManAI coilHead:
                EnemyPromptHandler.HandleCoilHead(coilHead, targetPlayer, willTeleportEnemy);
                break;

            case PufferAI sporeLizard:
                EnemyPromptHandler.HandleSporeLizard(sporeLizard, targetPlayer, willTeleportEnemy);
                break;

            case JesterAI jester:
                EnemyPromptHandler.HandleJester(jester, targetPlayer, willTeleportEnemy);
                break;

            case SandWormAI earthLeviathan:
                EnemyPromptHandler.HandleEarthLeviathan(earthLeviathan, targetPlayer, willTeleportEnemy);
                break;

            default:
                enemy.SetBehaviourState(BehaviourState.CHASE);
                break;
        }
    }
}

interface IEnemyPrompter;

static class EnemyPromptMixin {
    [RequireNamedArgs]
    internal static List<string> PromptEnemiesToTarget(
        this IEnemyPrompter _,
        PlayerControllerB targetPlayer,
        bool willTeleportEnemies = false
    ) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return [];

        List<string> enemyNames = [];

        Helper.Enemies.WhereIsNotNull().ForEach((enemy) => {
            if (enemy is DocileLocustBeesAI or DoublewingAI or BlobAI or DressGirlAI or LassoManAI or RadMechAI) return;

            enemy.targetPlayer = targetPlayer;
            enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
            enemy.SetMovingTowardsTargetPlayer(targetPlayer);
            enemyNames.Add(enemy.enemyType.enemyName);
            EnemyPromptHandler.HandleEnemy(enemy, targetPlayer, willTeleportEnemies);
        });

        return enemyNames;
    }
}
