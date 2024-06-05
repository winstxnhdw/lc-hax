#region

using System.Collections.Generic;
using GameNetcodeStuff;
using Hax;
using Unity.Netcode;
using UnityEngine;

#endregion

enum HoardingBugState {
    IDLE,
    SEARCHING_FOR_ITEMS,
    RETURNING_TO_NEST,
    CHASING_PLAYER,
    WATCHING_PLAYER,
    AT_NEST
}

class HoardingBugController : IEnemyController<HoarderBugAI> {
    static readonly HashSet<HoarderBugItem> _FakeStolenItems = [];

    internal bool angry = false;

    public void Update(HoarderBugAI enemy, bool isAIControlled) {
        if (isAIControlled) return;
        if (enemy.heldItem?.itemGrabbableObject is null) {
            if (this.angry) enemy.angryTimer = 15.0f;
            return;
        }

        enemy.angryTimer = 0.0f;
        enemy.SetBehaviourState(HoardingBugState.IDLE);
    }

    public void OnDeath(HoarderBugAI enemy) {
        if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) return;

        _ = enemy.Reflect().InvokeInternalMethod(
            "DropItemAndCallDropRPC",
            networkObject,
            false
        );
    }

    public void UsePrimarySkill(HoarderBugAI enemy) {
        if (enemy.heldItem is null && enemy.FindNearbyItem() is GrabbableObject grabbable)
            this.GrabItem(enemy, grabbable);

        else
            this.UseHeldItem(enemy);
    }

    public void UseSecondarySkill(HoarderBugAI enemy) {
        if (enemy.heldItem?.itemGrabbableObject is null) {
            if (!this.angry) {
                PlayerControllerB closePlayer = enemy.FindClosestPlayer();
                enemy.watchingPlayer = closePlayer;
                enemy.angryAtPlayer = closePlayer;
                enemy.angryTimer = 15.0f;
                this.angry = true;
                this.AttackPlayer(enemy, true);
                enemy.SetBehaviourState(HoardingBugState.CHASING_PLAYER);
            }
            else {
                enemy.angryAtPlayer = null;
                enemy.angryTimer = 0.0f;
                this.AttackPlayer(enemy, false);
                this.angry = false;
            }

            return;
        }

        if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject))
            _ = enemy.Reflect().InvokeInternalMethod("DropItemAndCallDropRPC", networkObject, false);
    }

    public void UseSpecialAbility(HoarderBugAI enemy) =>
        _ = RoundManager.PlayRandomClip(enemy.creatureVoice, enemy.chitterSFX, true, 1f, 0);

    public string GetPrimarySkillName(HoarderBugAI enemy) {
        if(enemy.heldItem?.itemGrabbableObject != null)
            return "Use item";
        else
            return "Grab item";
    }

    public string GetSecondarySkillName(HoarderBugAI enemy) {
        if (enemy.heldItem?.itemGrabbableObject != null) {
            return "Drop item";
        }
        else if (this.angry) {
            return "Stop Attack Mode";
        }
        else {
            return "Enter Attack Mode";
        }
    }

    public string GetSpecialAbilityName(HoarderBugAI _) => "Chitter";

    public void OnOutsideStatusChange(HoarderBugAI enemy) {
        enemy.StopSearch(enemy.searchForItems, true);
        enemy.StopSearch(enemy.searchForPlayer, true);
    }


    public void OnEnableAIControl(HoarderBugAI enemy, bool enabled){
        if (enabled) {
            enemy.Reflect().InvokeInternalMethod("ChooseNestPosition");
            enemy.SetBehaviourState(HoardingBugState.IDLE);
        }
        else
            enemy.nestPosition = new Vector3(2000f, 2000f, 2000f);
    }


    public void OnUnpossess(HoarderBugAI _) => this.angry = false;

    public void OnPossess(HoarderBugAI enemy) {
        this.angry = false;
        // calm the bug down when possessed
        enemy.angryTimer = 0.0f;
        enemy.SetBehaviourState(HoardingBugState.IDLE);
    }

    bool GetInChase(HoarderBugAI enemy) => enemy.Reflect().GetInternalField<bool>("inChase");

    float GettimeSinceHittingPlayer(HoarderBugAI enemy) =>
        enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");

    void SettimeSinceHittingPlayer(HoarderBugAI enemy, float value) =>
        enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);


    void UseHeldItem(HoarderBugAI enemy) {
        if (enemy.heldItem is not { itemGrabbableObject: GrabbableObject grabbable }) return;

        switch (grabbable) {
            case ShotgunItem gun:
                gun.ShootShotgun(enemy.transform);
                break;

            default:
                grabbable.InteractWithProp();
                break;
        }
    }

    void GrabItem(HoarderBugAI enemy, GrabbableObject item) {
        if (!item.TryGetComponent(out NetworkObject netItem)) return;

        _ = enemy.Reflect()
            .InvokeInternalMethod("GrabItem", netItem)?
            .SetInternalField("sendingGrabOrDropRPC", true);

        enemy.SwitchToBehaviourServerRpc(1);
        enemy.GrabItemServerRpc(netItem);
        item.EquipItem();
    }

    public void AttackPlayer(HoarderBugAI enemy, bool isAttacking) {
        if (!isAttacking) {
            foreach (HoarderBugItem fakeStolenItem in (HoarderBugItem[]) [.._FakeStolenItems]) {
                _FakeStolenItems.Remove(fakeStolenItem);
                HoarderBugAI.HoarderBugItems.Remove(fakeStolenItem);
            }

            return;
        }

        foreach (PlayerControllerB? playerControllerB in RoundManager.Instance.playersManager.allPlayerScripts)
        foreach (GrabbableObject? itemSlot in playerControllerB.ItemSlots) {
            if (itemSlot is null)
                continue;

            HoarderBugItem fakeStolenItem =
                new(itemSlot, HoarderBugItemStatus.Stolen, enemy.nestPosition);

            _ = _FakeStolenItems.Add(fakeStolenItem);

            HoarderBugAI.HoarderBugItems.Add(fakeStolenItem);
        }
    }
}
