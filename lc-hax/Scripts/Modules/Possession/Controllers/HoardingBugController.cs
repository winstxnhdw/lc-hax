using System.Collections.Generic;
using Hax;
using Unity.Netcode;
using UnityEngine;

internal enum HoardingBugState
{
    IDLE,
    SEARCHING_FOR_ITEMS,
    RETURNING_TO_NEST,
    CHASING_PLAYER,
    WATCHING_PLAYER,
    AT_NEST
}

internal class HoardingBugController : IEnemyController<HoarderBugAI>
{
    private static readonly HashSet<HoarderBugItem> _FakeStolenItems = [];

    internal bool angry = false;

    public void Update(HoarderBugAI enemy, bool isAIControlled)
    {
        if (isAIControlled) return;
        if (enemy.heldItem?.itemGrabbableObject is null)
        {
            if (angry) enemy.angryTimer = 15.0f;
            return;
        }

        enemy.angryTimer = 0.0f;
        enemy.SetBehaviourState(HoardingBugState.IDLE);
    }

    public void OnDeath(HoarderBugAI enemy)
    {
        if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) return;

        _ = enemy.Reflect().InvokeInternalMethod(
            "DropItemAndCallDropRPC",
            networkObject,
            false
        );
    }

    public void UsePrimarySkill(HoarderBugAI enemy)
    {
        if (enemy.heldItem is null && enemy.FindNearbyItem() is GrabbableObject grabbable)
            GrabItem(enemy, grabbable);

        else
            UseHeldItem(enemy);
    }

    public void UseSecondarySkill(HoarderBugAI enemy)
    {
        if (enemy.heldItem?.itemGrabbableObject is null)
        {
            if (!angry)
            {
                var closePlayer = enemy.FindClosestPlayer();
                enemy.watchingPlayer = closePlayer;
                enemy.angryAtPlayer = closePlayer;
                enemy.angryTimer = 15.0f;
                angry = true;
                AttackPlayer(enemy, true);
                enemy.SetBehaviourState(HoardingBugState.CHASING_PLAYER);
            }
            else
            {
                enemy.angryAtPlayer = null;
                enemy.angryTimer = 0.0f;
                AttackPlayer(enemy, false);
            }

            return;
        }

        if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject))
            _ = enemy.Reflect().InvokeInternalMethod("DropItemAndCallDropRPC", networkObject, false);
    }

    public void UseSpecialAbility(HoarderBugAI enemy)
    {
        _ = RoundManager.PlayRandomClip(enemy.creatureVoice, enemy.chitterSFX, true, 1f, 0);
    }

    public string GetPrimarySkillName(HoarderBugAI enemy)
    {
        return enemy.heldItem is not null ? "Use item" : "Grab Item";
    }

    public string GetSecondarySkillName(HoarderBugAI enemy)
    {
        return enemy.heldItem is null ? "" : "Drop item";
    }

    public void OnOutsideStatusChange(HoarderBugAI enemy)
    {
        enemy.StopSearch(enemy.searchForItems, true);
        enemy.StopSearch(enemy.searchForPlayer, true);
    }


    private void EnableAIControl(HoarderBugAI enemy, bool enabled)
    {
        if (enabled)
        {
            enemy.Reflect().InvokeInternalMethod("ChooseNestPosition");
            enemy.SetBehaviourState(HoardingBugState.IDLE);
        }
        else
        {
            enemy.nestPosition = new Vector3(2000f, 2000f, 2000f);
        }
    }


    private void OnUnpossess(HoarderBugAI _)
    {
        angry = false;
    }

    private void OnPossess(HoarderBugAI enemy)
    {
        angry = false;
        // calm the bug down when possessed
        enemy.angryTimer = 0.0f;
        enemy.SetBehaviourState(HoardingBugState.IDLE);
    }

    private bool GetInChase(HoarderBugAI enemy)
    {
        return enemy.Reflect().GetInternalField<bool>("inChase");
    }

    private float GettimeSinceHittingPlayer(HoarderBugAI enemy)
    {
        return enemy.Reflect().GetInternalField<float>("timeSinceHittingPlayer");
    }

    private void SettimeSinceHittingPlayer(HoarderBugAI enemy, float value)
    {
        enemy.Reflect().SetInternalField("timeSinceHittingPlayer", value);
    }


    private void UseHeldItem(HoarderBugAI enemy)
    {
        if (enemy.heldItem is not { itemGrabbableObject: GrabbableObject grabbable }) return;

        switch (grabbable)
        {
            case ShotgunItem gun:
                gun.ShootShotgun(enemy.transform);
                break;

            default:
                grabbable.InteractWithProp();
                break;
        }
    }

    private void GrabItem(HoarderBugAI enemy, GrabbableObject item)
    {
        if (!item.TryGetComponent(out NetworkObject netItem)) return;

        _ = enemy.Reflect()
            .InvokeInternalMethod("GrabItem", netItem)?
            .SetInternalField("sendingGrabOrDropRPC", true);

        enemy.SwitchToBehaviourServerRpc(1);
        enemy.GrabItemServerRpc(netItem);
        item.EquipItem();
    }

    public void AttackPlayer(HoarderBugAI enemy, bool isAttacking)
    {
        if (!isAttacking)
        {
            foreach (var fakeStolenItem in (HoarderBugItem[]) [.._FakeStolenItems])
            {
                _FakeStolenItems.Remove(fakeStolenItem);
                HoarderBugAI.HoarderBugItems.Remove(fakeStolenItem);
            }

            return;
        }

        foreach (var playerControllerB in RoundManager.Instance.playersManager.allPlayerScripts)
        foreach (var itemSlot in playerControllerB.ItemSlots)
        {
            if (itemSlot is null)
                continue;

            var fakeStolenItem = new HoarderBugItem(itemSlot, HoarderBugItemStatus.Stolen, enemy.nestPosition);

            _FakeStolenItems.Add(fakeStolenItem);

            HoarderBugAI.HoarderBugItems.Add(fakeStolenItem);
        }
    }
}