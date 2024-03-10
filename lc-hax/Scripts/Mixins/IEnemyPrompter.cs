using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


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
        enemy.SetOutsideStatus(!targetPlayer.isInsideFactory);
        enemy.transform.position = targetPlayer.transform.position;
        enemy.SyncPositionToClients();
    }

    bool IsEnemyAllowedOutside(EnemyAI enemy) => enemy is MaskedPlayerEnemy or DressGirlAI || enemy.enemyType.isOutsideEnemy;

    void HandleThumper(CrawlerAI thumper, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && this.IsEnemyAllowedOutside(thumper) && !targetPlayer.isInsideFactory) return;
        }
        this.TeleportEnemyToPlayer(thumper, targetPlayer, willTeleportEnemy, allowedInside: true);
        thumper.BeginChasingPlayerServerRpc(targetPlayer.PlayerIndex());
    }

    void HandleEyelessDog(MouthDogAI eyelessDog, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && !this.IsEnemyAllowedOutside(eyelessDog) && targetPlayer.isInsideFactory) return;
        }
        this.TeleportEnemyToPlayer(eyelessDog, targetPlayer, willTeleportEnemy, true);
        eyelessDog.ReactToOtherDogHowl(targetPlayer.transform.position);
    }

    void HandleBaboonHawk(BaboonBirdAI baboonHawk, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && !this.IsEnemyAllowedOutside(baboonHawk) && targetPlayer.isInsideFactory) return;
        }
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

    void HandleForestGiant(ForestGiantAI forestGiant, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && !this.IsEnemyAllowedOutside(forestGiant) && targetPlayer.isInsideFactory) return;
        }
        this.TeleportEnemyToPlayer(forestGiant, targetPlayer, willTeleportEnemy, true);
        forestGiant.SetBehaviourState(GiantState.CHASE);
        forestGiant.StopSearch(forestGiant.roamPlanet, false);
        forestGiant.chasingPlayer = targetPlayer;
        forestGiant.investigating = true;

        _ = forestGiant.SetDestinationToPosition(targetPlayer.transform.position);
        _ = forestGiant.Reflect().SetInternalField("lostPlayerInChase", false);
    }

    void HandleSnareFlea(CentipedeAI snareFlea, PlayerControllerB targetPlayer) {
        if (!targetPlayer.isInsideFactory) return;
        snareFlea.targetPlayer = targetPlayer;
        snareFlea.SetBehaviourState(SnareFleaState.CHASING);
    }

    void HandleBracken(FlowermanAI bracken, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && this.IsEnemyAllowedOutside(bracken) && !targetPlayer.isInsideFactory) return;
        }
        this.TeleportEnemyToPlayer(bracken, targetPlayer, willTeleportEnemy, allowedInside: true);
        bracken.SetBehaviourState(BrackenState.ANGER);
        bracken.EnterAngerModeServerRpc(float.MaxValue);
    }

    void HandleBunkerSpider(SandSpiderAI bunkerSpider, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && this.IsEnemyAllowedOutside(bunkerSpider) && !targetPlayer.isInsideFactory) return;
        }
        this.TeleportEnemyToPlayer(bunkerSpider, targetPlayer, willTeleportEnemy, allowedInside: true);
        if (willTeleportEnemy) {
            bunkerSpider.meshContainerPosition = targetPlayer.transform.position;
            bunkerSpider.SyncMeshContainerPositionToClients();
        }
        bunkerSpider.SwitchToBehaviourServerRpc(2);
        bunkerSpider.TriggerChaseWithPlayer(targetPlayer);
        Vector3 playerPosition = targetPlayer.transform.position;

        bunkerSpider.SpawnWebTrapServerRpc(
            playerPosition,
            playerPosition + (targetPlayer.transform.forward * 5.0f)
        );
        
        _ = bunkerSpider.Reflect()
                        .SetInternalField("watchFromDistance", false)?
                        .SetInternalField("chaseTimer", float.MaxValue);
    }

    void HandleBee(RedLocustBees bee, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && !this.IsEnemyAllowedOutside(bee) && targetPlayer.isInsideFactory) return;
        }
        this.TeleportEnemyToPlayer(bee, targetPlayer, willTeleportEnemy, true);
        bee.SetBehaviourState(BeesState.ATTACK);
        bee.EnterAttackZapModeServerRpc(targetPlayer.PlayerIndex());
    }

    void HandleHoardingBug(HoarderBugAI hoardingBug, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && this.IsEnemyAllowedOutside(hoardingBug) && !targetPlayer.isInsideFactory) return;
        }

        this.TeleportEnemyToPlayer(hoardingBug, targetPlayer, willTeleportEnemy, allowedInside: true);
        hoardingBug.SetBehaviourState(HoardingBugState.CHASING_PLAYER);
        hoardingBug.angryAtPlayer = targetPlayer;
        hoardingBug.angryTimer = float.MaxValue;

        _ = hoardingBug.Reflect()
                       .SetInternalField("lostPlayerInChase", false)?
                       .InvokeInternalMethod("SyncNestPositionServerRpc", targetPlayer.transform.position);
    }

    void HandleNutcracker(NutcrackerEnemyAI nutcracker, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && this.IsEnemyAllowedOutside(nutcracker) && !targetPlayer.isInsideFactory) return;
        }

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
        maskedPlayer.SwitchToBehaviourServerRpc(1);
        maskedPlayer.targetPlayer = targetPlayer;
        maskedPlayer.SetMovingTowardsTargetPlayer(targetPlayer);
        maskedPlayer.SetRunningServerRpc(true);
        maskedPlayer.SetEnemyOutside(!targetPlayer.isInsideFactory);
    }

    void HandleCoilHead(SpringManAI coilHead, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && this.IsEnemyAllowedOutside(coilHead) && !targetPlayer.isInsideFactory) return;
        }
        this.TeleportEnemyToPlayer(coilHead, targetPlayer, willTeleportEnemy, allowedInside: true);
        coilHead.SetBehaviourState(CoilHeadState.Chase);
        coilHead.SetAnimationGoServerRpc();
        coilHead.creatureAnimator.SetFloat("walkSpeed", 5.0f);
        coilHead.mainCollider.isTrigger = true;
        coilHead.agent.speed = 5.0f;
    }

    void HandleSporeLizard(PufferAI sporeLizard, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && this.IsEnemyAllowedOutside(sporeLizard) && !targetPlayer.isInsideFactory) return;
        }
        this.TeleportEnemyToPlayer(sporeLizard, targetPlayer, willTeleportEnemy, allowedInside: true);
        sporeLizard.targetPlayer = targetPlayer;
        sporeLizard.SetMovingTowardsTargetPlayer(targetPlayer);
        sporeLizard.SetBehaviourState(SporeLizardState.HOSTILE);
    }

    void HandleJester(JesterAI jester, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!willTeleportEnemy) {
            if (!overrideInsideFactory && this.IsEnemyAllowedOutside(jester) && !targetPlayer.isInsideFactory) return;
        }
        this.TeleportEnemyToPlayer(jester, targetPlayer, willTeleportEnemy, allowedInside: true);
        jester.targetPlayer = targetPlayer;
        _ = jester.Reflect().SetInternalField("previousState", (int)JesterState.CRANKING);
        jester.SetBehaviourState(JesterState.OPEN);
        jester.popUpTimer = 0.0f;
        jester.SetMovingTowardsTargetPlayer(targetPlayer);
        _ = jester.Reflect().SetInternalField("noPlayersToChaseTimer", 20f);
    }

    void HandleEarthLeviathan(SandWormAI earthLeviathan, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(earthLeviathan, targetPlayer, willTeleportEnemy, true);
        earthLeviathan.SetBehaviourState(BehaviourState.CHASE);
    }

    void HandleDressGirl(DressGirlAI dressGirl, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(dressGirl, targetPlayer, willTeleportEnemy, true);
        dressGirl.hauntingPlayer = targetPlayer;
        dressGirl.ChangeEnemyOwnerServerRpc(targetPlayer.playerClientId);
        dressGirl.SetBehaviourState(BehaviourState.IDLE);
    }

    void HandleDoublewingBird(DoublewingAI doublewingBird, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(doublewingBird, targetPlayer, willTeleportEnemy, true);
    }

    void HandleDocileLocustBees(DocileLocustBeesAI docileLocustBees, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(docileLocustBees, targetPlayer, willTeleportEnemy, true);
        docileLocustBees.SetBehaviourState(BehaviourState.IDLE);
    }

    internal void HandleEnemy(EnemyAI enemy, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        switch (enemy) {
            case CrawlerAI thumper:
                this.HandleThumper(thumper, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case MouthDogAI eyelessDog:
                this.HandleEyelessDog(eyelessDog, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case BaboonBirdAI baboonHawk:
                this.HandleBaboonHawk(baboonHawk, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case ForestGiantAI forestGiant:
                this.HandleForestGiant(forestGiant, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case CentipedeAI snareFlea:
                this.HandleSnareFlea(snareFlea, targetPlayer);
                break;

            case FlowermanAI bracken:
                this.HandleBracken(bracken, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case SandSpiderAI bunkerSpider:
                this.HandleBunkerSpider(bunkerSpider, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case RedLocustBees bee:
                this.HandleBee(bee, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case HoarderBugAI hoardingBug:
                this.HandleHoardingBug(hoardingBug, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case NutcrackerEnemyAI nutcracker:
                this.HandleNutcracker(nutcracker, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case MaskedPlayerEnemy maskedPlayer:
                this.HandleMaskedPlayer(maskedPlayer, targetPlayer, willTeleportEnemy);
                break;

            case SpringManAI coilHead:
                this.HandleCoilHead(coilHead, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case PufferAI sporeLizard:
                this.HandleSporeLizard(sporeLizard, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case JesterAI jester:
                this.HandleJester(jester, targetPlayer, willTeleportEnemy, overrideInsideFactory);
                break;

            case SandWormAI earthLeviathan:
                this.HandleEarthLeviathan(earthLeviathan, targetPlayer, willTeleportEnemy);
                break;

            case DressGirlAI dressGirl:
                this.HandleDressGirl(dressGirl, targetPlayer, willTeleportEnemy);
                break;

            case DoublewingAI doublewingBird:
                this.HandleDoublewingBird(doublewingBird, targetPlayer, willTeleportEnemy);
                break;

            case DocileLocustBeesAI docileLocustBees:
                this.HandleDocileLocustBees(docileLocustBees, targetPlayer, willTeleportEnemy);
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
        bool willTeleportEnemies = false,
        bool overrideInsideFactory = false
    ) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return [];

        List<string> enemyNames = [];
        EnemyPromptHandler enemyPromptHandler = new();

        Helper.Enemies.WhereIsNotNull().ForEach((enemy) => {
            if (enemy is DocileLocustBeesAI or DoublewingAI or BlobAI or TestEnemy or LassoManAI) return;

            enemy.targetPlayer = targetPlayer;
            enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
            enemy.SetMovingTowardsTargetPlayer(targetPlayer);
            enemyNames.Add(enemy.enemyType.enemyName);
            enemyPromptHandler.HandleEnemy(enemy, targetPlayer, willTeleportEnemies, overrideInsideFactory);
        });

        return enemyNames;
    }
}
