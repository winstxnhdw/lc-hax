using System.Collections.Generic;
using UnityEngine;
using GameNetcodeStuff;

enum BehaviourState {
    IDLE = 0,
    CHASE = 1,
    AGGRAVATED = 2,
    UNKNOWN = 3
}

class EnemyPromptHandler {
    void TeleportEnemyToPlayer(
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

    void HandleThumper(CrawlerAI thumper, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(thumper, targetPlayer, willTeleportEnemy, allowedInside: true);
        thumper.BeginChasingPlayerServerRpc(targetPlayer.PlayerIndex());
    }

    void HandleEyelessDog(MouthDogAI eyelessDog, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(eyelessDog, targetPlayer, willTeleportEnemy, true);
        eyelessDog.ReactToOtherDogHowl(targetPlayer.transform.position);
    }

    void HandleBaboonHawk(BaboonBirdAI baboonHawk, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(baboonHawk, targetPlayer, willTeleportEnemy, true);

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

    void HandleForestGiant(ForestGiantAI forestGiant, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(forestGiant, targetPlayer, willTeleportEnemy, true);
        forestGiant.SetBehaviourState(BehaviourState.CHASE);
        forestGiant.StopSearch(forestGiant.roamPlanet, false);
        forestGiant.chasingPlayer = targetPlayer;
        forestGiant.investigating = true;

        _ = forestGiant.SetDestinationToPosition(targetPlayer.transform.position);
        _ = forestGiant.Reflect().SetInternalField("lostPlayerInChase", false);
    }

    void HandleSnareFlea(CentipedeAI snareFlea, PlayerControllerB targetPlayer) {
        if (!targetPlayer.isInsideFactory) return;
        snareFlea.SetBehaviourState(BehaviourState.CHASE);
    }

    void HandleBracken(FlowermanAI bracken, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(bracken, targetPlayer, willTeleportEnemy, allowedInside: true);
        bracken.SetBehaviourState(BehaviourState.AGGRAVATED);
        bracken.EnterAngerModeServerRpc(float.MaxValue);
    }

    void HandleBunkerSpider(SandSpiderAI bunkerSpider, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(bunkerSpider, targetPlayer, willTeleportEnemy, allowedInside: true);
        bunkerSpider.SetBehaviourState(BehaviourState.AGGRAVATED);

        Vector3 playerPosition = targetPlayer.transform.position;

        bunkerSpider.SpawnWebTrapServerRpc(
            playerPosition,
            playerPosition + (targetPlayer.transform.forward * 5.0f)
        );

        _ = bunkerSpider.Reflect()
                        .SetInternalField("watchFromDistance", false)?
                        .SetInternalField("chaseTimer", float.MaxValue);
    }

    void HandleBee(RedLocustBees bee, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(bee, targetPlayer, willTeleportEnemy, true);
        bee.SetBehaviourState(BehaviourState.AGGRAVATED);
    }

    void HandleHoardingBug(HoarderBugAI hoardingBug, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(hoardingBug, targetPlayer, willTeleportEnemy, allowedInside: true);
        hoardingBug.SetBehaviourState(BehaviourState.AGGRAVATED);
        hoardingBug.angryAtPlayer = targetPlayer;
        hoardingBug.angryTimer = float.MaxValue;

        _ = hoardingBug.Reflect()
                       .SetInternalField("lostPlayerInChase", false)?
                       .InvokeInternalMethod("SyncNestPositionServerRpc", targetPlayer.transform.position);
    }

    void HandleNutcracker(NutcrackerEnemyAI nutcracker, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(nutcracker, targetPlayer, willTeleportEnemy, true, true);

        nutcracker.StopInspection();
        nutcracker.SeeMovingThreatServerRpc(targetPlayer.PlayerIndex());
        nutcracker.AimGunServerRpc(targetPlayer.transform.position);

        _ = nutcracker.Reflect()
                      .SetInternalField("lastSeenPlayerPos", targetPlayer.transform.position)?
                      .SetInternalField("timeSinceSeeingTarget", 0);
    }

    void HandleMaskedPlayer(MaskedPlayerEnemy maskedPlayer, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(maskedPlayer, targetPlayer, willTeleportEnemy, true, true);
        maskedPlayer.SetBehaviourState(BehaviourState.CHASE);
        maskedPlayer.SetEnemyOutside(!targetPlayer.isInsideFactory);
    }

    void HandleCoilHead(SpringManAI coilHead, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(coilHead, targetPlayer, willTeleportEnemy, allowedInside: true);
        coilHead.SetBehaviourState(BehaviourState.CHASE);
        coilHead.SetAnimationGoServerRpc();
        coilHead.creatureAnimator.SetFloat("walkSpeed", 5.0f);
        coilHead.mainCollider.isTrigger = true;
        coilHead.agent.speed = 5.0f;
    }

    void HandleSporeLizard(PufferAI sporeLizard, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(sporeLizard, targetPlayer, willTeleportEnemy, allowedInside: true);
        sporeLizard.SetBehaviourState(BehaviourState.AGGRAVATED);
    }

    void HandleJester(JesterAI jester, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(jester, targetPlayer, willTeleportEnemy, allowedInside: true);
        jester.SetBehaviourState(BehaviourState.AGGRAVATED);
    }

    void HandleEarthLeviathan(SandWormAI earthLeviathan, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(earthLeviathan, targetPlayer, willTeleportEnemy, true);
        earthLeviathan.SetBehaviourState(BehaviourState.CHASE);
    }

    internal void HandleEnemy(EnemyAI enemy, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        switch (enemy) {
            case CrawlerAI thumper:
                this.HandleThumper(thumper, targetPlayer, willTeleportEnemy);
                break;

            case MouthDogAI eyelessDog:
                this.HandleEyelessDog(eyelessDog, targetPlayer, willTeleportEnemy);
                break;

            case BaboonBirdAI baboonHawk:
                this.HandleBaboonHawk(baboonHawk, targetPlayer, willTeleportEnemy);
                break;

            case ForestGiantAI forestGiant:
                this.HandleForestGiant(forestGiant, targetPlayer, willTeleportEnemy);
                break;

            case CentipedeAI snareFlea:
                this.HandleSnareFlea(snareFlea, targetPlayer);
                break;

            case FlowermanAI bracken:
                this.HandleBracken(bracken, targetPlayer, willTeleportEnemy);
                break;

            case SandSpiderAI bunkerSpider:
                this.HandleBunkerSpider(bunkerSpider, targetPlayer, willTeleportEnemy);
                break;

            case RedLocustBees bee:
                this.HandleBee(bee, targetPlayer, willTeleportEnemy);
                break;

            case HoarderBugAI hoardingBug:
                this.HandleHoardingBug(hoardingBug, targetPlayer, willTeleportEnemy);
                break;

            case NutcrackerEnemyAI nutcracker:
                this.HandleNutcracker(nutcracker, targetPlayer, willTeleportEnemy);
                break;

            case MaskedPlayerEnemy maskedPlayer:
                this.HandleMaskedPlayer(maskedPlayer, targetPlayer, willTeleportEnemy);
                break;

            case SpringManAI coilHead:
                this.HandleCoilHead(coilHead, targetPlayer, willTeleportEnemy);
                break;

            case PufferAI sporeLizard:
                this.HandleSporeLizard(sporeLizard, targetPlayer, willTeleportEnemy);
                break;

            case JesterAI jester:
                this.HandleJester(jester, targetPlayer, willTeleportEnemy);
                break;

            case SandWormAI earthLeviathan:
                this.HandleEarthLeviathan(earthLeviathan, targetPlayer, willTeleportEnemy);
                break;

            default:
                enemy.SetBehaviourState(BehaviourState.CHASE);
                break;
        }
    }
}

interface IEnemyPrompter { }

static class EnemyPromptMixin {
    [RequireNamedArgs]
    internal static List<string> PromptEnemiesToTarget(
        this IEnemyPrompter _,
        PlayerControllerB targetPlayer,
        bool willTeleportEnemies = false
    ) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return [];

        List<string> enemyNames = [];
        EnemyPromptHandler enemyPromptHandler = new();

        Helper.Enemies.WhereIsNotNull().ForEach((enemy) => {
            if (enemy is DocileLocustBeesAI or DoublewingAI or BlobAI or DressGirlAI or LassoManAI or RadMechAI) return;

            enemy.targetPlayer = targetPlayer;
            enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
            enemy.SetMovingTowardsTargetPlayer(targetPlayer);
            enemyNames.Add(enemy.enemyType.enemyName);
            enemyPromptHandler.HandleEnemy(enemy, targetPlayer, willTeleportEnemies);
        });

        return enemyNames;
    }
}
