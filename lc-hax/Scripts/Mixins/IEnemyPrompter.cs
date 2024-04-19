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
        enemy.TakeOwnerShipIfNotOwned();
        enemy.transform.position = targetPlayer.transform.position;
        enemy.SetOutsideStatus(!targetPlayer.isInsideFactory);
        enemy.SyncPositionToClients();
    }

    bool IsEnemyAllowedInside(EnemyAI enemy, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (overrideInsideFactory) return true;
        if (willTeleportEnemy) return true;
        if (enemy is MaskedPlayerEnemy or DressGirlAI) return true;
        return !targetPlayer.isInsideFactory;

    }

    bool IsEnemyAllowedOutside(EnemyAI enemy, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (overrideInsideFactory) return true;
        if (willTeleportEnemy) return true;
        if (enemy is MaskedPlayerEnemy or DressGirlAI) return true;
        return targetPlayer.isInsideFactory;
    }

    bool HandleThumper(CrawlerAI thumper, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if(!this.IsEnemyAllowedOutside(thumper, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(thumper, targetPlayer, willTeleportEnemy, allowedInside: true);
        thumper.TakeOwnership();
        thumper.targetPlayer = targetPlayer;
        thumper.SetMovingTowardsTargetPlayer(targetPlayer);
        thumper.BeginChasingPlayerServerRpc(targetPlayer.PlayerIndex());
        return true;
    }

    bool HandleEyelessDog(MouthDogAI eyelessDog, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedInside(eyelessDog, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(eyelessDog, targetPlayer, willTeleportEnemy, true);
        eyelessDog.TakeOwnership();
        eyelessDog.targetPlayer = targetPlayer;
        eyelessDog.SetMovingTowardsTargetPlayer(targetPlayer);
        eyelessDog.ReactToOtherDogHowl(targetPlayer.transform.position);
        return true;
    }

    bool HandleBaboonHawk(BaboonBirdAI baboonHawk, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedInside(baboonHawk, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(baboonHawk, targetPlayer, willTeleportEnemy, true);
        baboonHawk.TakeOwnership();
        baboonHawk.targetPlayer = targetPlayer;
        baboonHawk.SetMovingTowardsTargetPlayer(targetPlayer);

        baboonHawk.SetAggressiveModeServerRpc(1);
        _ = baboonHawk.Reflect().InvokeInternalMethod("ReactToThreat", targetPlayer.ToThreat());
        return true;
    }

    bool HandleForestGiant(ForestGiantAI forestGiant, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedInside(forestGiant, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(forestGiant, targetPlayer, willTeleportEnemy, true);
        forestGiant.TakeOwnership();
        forestGiant.targetPlayer = targetPlayer;
        forestGiant.SetBehaviourState(GiantState.CHASE);
        forestGiant.StopSearch(forestGiant.roamPlanet, false);
        forestGiant.chasingPlayer = targetPlayer;
        forestGiant.investigating = true;
        forestGiant.SetMovingTowardsTargetPlayer(targetPlayer);

        _ = forestGiant.SetDestinationToPosition(targetPlayer.transform.position);
        _ = forestGiant.Reflect().SetInternalField("lostPlayerInChase", false);
        return true;
    }

    bool HandleSnareFlea(CentipedeAI snareFlea, PlayerControllerB targetPlayer) {
        if (!targetPlayer.isInsideFactory) return false;
        snareFlea.TakeOwnership();
        snareFlea.targetPlayer = targetPlayer;
        snareFlea.SetMovingTowardsTargetPlayer(targetPlayer);
        snareFlea.targetPlayer = targetPlayer;
        snareFlea.SetBehaviourState(SnareFleaState.CHASING);
        return true;
    }

    bool HandleBracken(FlowermanAI bracken, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedOutside(bracken, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(bracken, targetPlayer, willTeleportEnemy, allowedInside: true);
        bracken.TakeOwnership();
        bracken.targetPlayer = targetPlayer;
        bracken.SetMovingTowardsTargetPlayer(targetPlayer);
        bracken.SetBehaviourState(BrackenState.ANGER);
        bracken.EnterAngerModeServerRpc(float.MaxValue);
        return true;
    }

    bool HandleBunkerSpider(SandSpiderAI bunkerSpider, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedOutside(bunkerSpider, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(bunkerSpider, targetPlayer, willTeleportEnemy, allowedInside: true);
        bunkerSpider.TakeOwnership();
        bunkerSpider.targetPlayer = targetPlayer;
        bunkerSpider.SetMovingTowardsTargetPlayer(targetPlayer);
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
        return true;
    }

    bool HandleBee(RedLocustBees bee, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedInside(bee, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(bee, targetPlayer, willTeleportEnemy, true);
        bee.TakeOwnership();
        bee.targetPlayer = targetPlayer;
        bee.SetMovingTowardsTargetPlayer(targetPlayer);
        bee.SetBehaviourState(BeesState.ATTACK);
        bee.EnterAttackZapModeServerRpc(targetPlayer.PlayerIndex());
        return true;
    }

    bool HandleHoardingBug(HoarderBugAI hoardingBug, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedOutside(hoardingBug, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(hoardingBug, targetPlayer, willTeleportEnemy, allowedInside: true);
        hoardingBug.TakeOwnership();
        hoardingBug.targetPlayer = targetPlayer;
        hoardingBug.SetMovingTowardsTargetPlayer(targetPlayer);
        hoardingBug.SetBehaviourState(HoardingBugState.CHASING_PLAYER);
        hoardingBug.angryAtPlayer = targetPlayer;
        hoardingBug.angryTimer = float.MaxValue;

        _ = hoardingBug.Reflect()
                       .SetInternalField("lostPlayerInChase", false)?
                       .InvokeInternalMethod("SyncNestPositionServerRpc", targetPlayer.transform.position);
        return true;
    }

    bool HandleNutcracker(NutcrackerEnemyAI nutcracker, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedOutside(nutcracker, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;

        this.TeleportEnemyToPlayer(nutcracker, targetPlayer, willTeleportEnemy, true, true);
        nutcracker.TakeOwnership();
        nutcracker.targetPlayer = targetPlayer;
        nutcracker.SetMovingTowardsTargetPlayer(targetPlayer);

        nutcracker.StopInspection();
        nutcracker.SeeMovingThreatServerRpc(targetPlayer.PlayerIndex());
        nutcracker.AimGunServerRpc(targetPlayer.transform.position);

        _ = nutcracker.Reflect()
                      .SetInternalField("lastSeenPlayerPos", targetPlayer.transform.position)?
                      .SetInternalField("timeSinceSeeingTarget", 0);
        return true;
    }

    bool HandleMaskedPlayer(MaskedPlayerEnemy maskedPlayer, PlayerControllerB targetPlayer, bool willTeleportEnemy) {

        this.TeleportEnemyToPlayer(maskedPlayer, targetPlayer, willTeleportEnemy, true, true);
        maskedPlayer.TakeOwnership();
        maskedPlayer.targetPlayer = targetPlayer;
        maskedPlayer.SetMovingTowardsTargetPlayer(targetPlayer);
        maskedPlayer.SwitchToBehaviourServerRpc(1);
        maskedPlayer.SetRunningServerRpc(true);
        maskedPlayer.SetEnemyOutside(!targetPlayer.isInsideFactory);
        maskedPlayer.targetPlayer = targetPlayer;
        maskedPlayer.SetMovingTowardsTargetPlayer(targetPlayer);
        return true;
    }

    bool HandleCoilHead(SpringManAI coilHead, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedOutside(coilHead, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(coilHead, targetPlayer, willTeleportEnemy, allowedInside: true);
        coilHead.TakeOwnership();
        coilHead.targetPlayer = targetPlayer;
        coilHead.SetMovingTowardsTargetPlayer(targetPlayer);
        coilHead.SetBehaviourState(CoilHeadState.Chase);
        coilHead.SetAnimationGoServerRpc();
        coilHead.creatureAnimator.SetFloat("walkSpeed", 5.0f);
        coilHead.mainCollider.isTrigger = true;
        coilHead.agent.speed = 5.0f;
        return true;
    }

    bool HandleSporeLizard(PufferAI sporeLizard, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedOutside(sporeLizard, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(sporeLizard, targetPlayer, willTeleportEnemy, allowedInside: true);
        sporeLizard.TakeOwnership();
        sporeLizard.targetPlayer = targetPlayer;
        sporeLizard.SetMovingTowardsTargetPlayer(targetPlayer);
        sporeLizard.SetMovingTowardsTargetPlayer(targetPlayer);
        sporeLizard.SetBehaviourState(SporeLizardState.HOSTILE);
        return true;
    }

    bool HandleJester(JesterAI jester, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedOutside(jester, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(jester, targetPlayer, willTeleportEnemy, allowedInside: true);
        jester.TakeOwnership();
        jester.targetPlayer = targetPlayer;
        jester.SetMovingTowardsTargetPlayer(targetPlayer);
        _ = jester.Reflect().SetInternalField("previousState", (int)JesterState.CRANKING);
        jester.SetBehaviourState(JesterState.OPEN);
        jester.popUpTimer = 0.0f;
        jester.SetMovingTowardsTargetPlayer(targetPlayer);
        _ = jester.Reflect().SetInternalField("noPlayersToChaseTimer", 20f);
        return true;
    }

    bool HandleEarthLeviathan(SandWormAI earthLeviathan, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        if (!this.IsEnemyAllowedInside(earthLeviathan, targetPlayer, willTeleportEnemy, false)) return false;
        this.TeleportEnemyToPlayer(earthLeviathan, targetPlayer, willTeleportEnemy, true);
        earthLeviathan.TakeOwnership();
        earthLeviathan.targetPlayer = targetPlayer;
        earthLeviathan.SetMovingTowardsTargetPlayer(targetPlayer);
        earthLeviathan.SetBehaviourState(BehaviourState.CHASE);
        return true;
    }

    bool HandleDressGirl(DressGirlAI dressGirl, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(dressGirl, targetPlayer, willTeleportEnemy, true);
        dressGirl.TakeOwnership();
        dressGirl.targetPlayer = targetPlayer;
        dressGirl.hauntingPlayer = targetPlayer;
        dressGirl.SetMovingTowardsTargetPlayer(targetPlayer);
        dressGirl.SetOwner(targetPlayer);
        dressGirl.SetBehaviourState(BehaviourState.IDLE);
        return true;
    }
    bool HandleRadMech(RadMechAI radMech, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedInside(radMech, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(radMech, targetPlayer, willTeleportEnemy, true);
        radMech.TakeOwnership();
        radMech.targetPlayer = targetPlayer;
        radMech.targetedThreat = targetPlayer.ToThreat();
        //radMech.SetBehaviourState(RadMechBehaviorState.Alert);
        //radMech.SetMovingTowardsTargetPlayer(targetPlayer);
        _ = radMech.SetDestinationToPosition(targetPlayer.transform.position);
        radMech.SetLostCreatureInChaseClientRpc(false);
       _ = radMech.Reflect().SetInternalField("losTimer", 100f);
        _ = radMech.Reflect().SetInternalField("lostCreatureInChase", false);
        _ = radMech.Reflect().SetInternalField("lostCreatureInChaseDebounce", false);

        return true;
    }

    bool HandleButler(ButlerEnemyAI butler, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedOutside(butler, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(butler, targetPlayer, willTeleportEnemy, true);
        butler.TakeOwnership();
        butler.targetPlayer = targetPlayer;
        butler.SetMovingTowardsTargetPlayer(targetPlayer);
        butler.SetBehaviourState(ButlerBehaviorState.Alert);
        butler.SetOwner(targetPlayer);
        return true;
    }
    bool HandleButlerBees(ButlerBeesEnemyAI butlerBees, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        if (!this.IsEnemyAllowedOutside(butlerBees, targetPlayer, willTeleportEnemy, overrideInsideFactory)) return false;
        this.TeleportEnemyToPlayer(butlerBees, targetPlayer, willTeleportEnemy, true);
        butlerBees.TakeOwnership();
        butlerBees.targetPlayer = targetPlayer;
        butlerBees.SetMovingTowardsTargetPlayer(targetPlayer);
        butlerBees.SetOwner(targetPlayer);
        return true;
    }


    bool HandleDoublewingBird(DoublewingAI doublewingBird, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        if(this.IsEnemyAllowedInside(doublewingBird, targetPlayer, willTeleportEnemy, false)) return false;
        this.TeleportEnemyToPlayer(doublewingBird, targetPlayer, willTeleportEnemy, true);
        doublewingBird.TakeOwnership();
        doublewingBird.SetMovingTowardsTargetPlayer(targetPlayer);
        return true;
    }

    bool HandleDocileLocustBees(DocileLocustBeesAI docileLocustBees, PlayerControllerB targetPlayer, bool willTeleportEnemy) {
        this.TeleportEnemyToPlayer(docileLocustBees, targetPlayer, willTeleportEnemy, true);
        docileLocustBees.TakeOwnership();
        docileLocustBees.SetMovingTowardsTargetPlayer(targetPlayer);
        docileLocustBees.SetBehaviourState(BehaviourState.IDLE);
        return true;
    }

    internal bool HandleEnemy(EnemyAI enemy, PlayerControllerB targetPlayer, bool willTeleportEnemy, bool overrideInsideFactory) {
        switch (enemy) {

            #region Special Enemies
            case MaskedPlayerEnemy maskedPlayer:
                return this.HandleMaskedPlayer(maskedPlayer, targetPlayer, willTeleportEnemy);

            case DressGirlAI dressGirl:
                return this.HandleDressGirl(dressGirl, targetPlayer, willTeleportEnemy);


            #endregion

            #region Outside Enemies

            case MouthDogAI eyelessDog:
                return this.HandleEyelessDog(eyelessDog, targetPlayer, willTeleportEnemy, overrideInsideFactory);

            case BaboonBirdAI baboonHawk:
                return this.HandleBaboonHawk(baboonHawk, targetPlayer, willTeleportEnemy, overrideInsideFactory);

            case ForestGiantAI forestGiant:
                return this.HandleForestGiant(forestGiant, targetPlayer, willTeleportEnemy, overrideInsideFactory);

            case RadMechAI RadMech:
                return this.HandleRadMech(RadMech, targetPlayer, willTeleportEnemy, overrideInsideFactory);

            case RedLocustBees bee:
                return this.HandleBee(bee, targetPlayer, willTeleportEnemy, overrideInsideFactory);

            case SandWormAI earthLeviathan:
                return this.HandleEarthLeviathan(earthLeviathan, targetPlayer, willTeleportEnemy);

            case DoublewingAI doublewingBird:
                return this.HandleDoublewingBird(doublewingBird, targetPlayer, willTeleportEnemy);

            case DocileLocustBeesAI docileLocustBees:
                return this.HandleDocileLocustBees(docileLocustBees, targetPlayer, willTeleportEnemy);

            #endregion

            #region Inside Enemies
            case CrawlerAI thumper:
                return this.HandleThumper(thumper, targetPlayer, willTeleportEnemy, overrideInsideFactory);
            case CentipedeAI snareFlea:
                return this.HandleSnareFlea(snareFlea, targetPlayer);
            case FlowermanAI bracken:
                return this.HandleBracken(bracken, targetPlayer, willTeleportEnemy, overrideInsideFactory);
            case ButlerEnemyAI butler:
                return this.HandleButler(butler, targetPlayer, willTeleportEnemy, overrideInsideFactory);
            case SandSpiderAI bunkerSpider:
                return this.HandleBunkerSpider(bunkerSpider, targetPlayer, willTeleportEnemy, overrideInsideFactory);
            case HoarderBugAI hoardingBug:
                return this.HandleHoardingBug(hoardingBug, targetPlayer, willTeleportEnemy, overrideInsideFactory);
            case ButlerBeesEnemyAI butlerbees:
                return this.HandleButlerBees(butlerbees, targetPlayer, willTeleportEnemy, overrideInsideFactory);
            case NutcrackerEnemyAI nutcracker:
                return this.HandleNutcracker(nutcracker, targetPlayer, willTeleportEnemy, overrideInsideFactory);
            case SpringManAI coilHead:
                return this.HandleCoilHead(coilHead, targetPlayer, willTeleportEnemy, overrideInsideFactory);
            case PufferAI sporeLizard:
                return this.HandleSporeLizard(sporeLizard, targetPlayer, willTeleportEnemy, overrideInsideFactory);
            case JesterAI jester:
                return this.HandleJester(jester, targetPlayer, willTeleportEnemy, overrideInsideFactory);
            #endregion

            default:
                if (enemy.enemyType.isOutsideEnemy && !targetPlayer.isInsideFactory) {
                    this.TeleportEnemyToPlayer(enemy, targetPlayer, willTeleportEnemy, true);
                    enemy.TakeOwnership();
                    enemy.targetPlayer = targetPlayer;
                    enemy.SetMovingTowardsTargetPlayer(targetPlayer);
                    enemy.SetBehaviourState(BehaviourState.CHASE);
                    return true;
                }
                if (!enemy.enemyType.isOutsideEnemy && targetPlayer.isInsideFactory) {
                    this.TeleportEnemyToPlayer(enemy, targetPlayer, willTeleportEnemy, true);
                    enemy.TakeOwnership();
                    enemy.targetPlayer = targetPlayer;
                    enemy.SetMovingTowardsTargetPlayer(targetPlayer);
                    enemy.SetBehaviourState(BehaviourState.CHASE);
                    return true;
                }
                return false;
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

            if (enemyPromptHandler.HandleEnemy(enemy, targetPlayer, willTeleportEnemies, overrideInsideFactory)) {
                enemyNames.Add(enemy.enemyType.enemyName);
            }
        });

        return enemyNames;
    }
}
