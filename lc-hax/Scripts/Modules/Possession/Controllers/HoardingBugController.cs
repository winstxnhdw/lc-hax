using Unity.Netcode;
using GameNetcodeStuff;
using Hax;

enum HoardingBugState {
    IDLE,
    SEARCHING,
    RETURNING,
    CHASING,
    WATCHING,
    NESTED
}

class HoardingBugController : IEnemyController<HoarderBugAI> {
    void UseHeldItem(HoarderBugAI enemy) {
        if (enemy.heldItem is not { itemGrabbableObject: GrabbableObject grabbable }) return;

        switch (grabbable) {
            case ShotgunItem gun:
                gun.ShootShotgun(enemy.transform);
                break;

            default:
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
    }

    public void Update(HoarderBugAI enemy, bool isAIControlled) {
        if (isAIControlled) return;
        if (enemy.heldItem?.itemGrabbableObject is null) return;

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
        if (enemy.heldItem is null && enemy.FindNearbyItem() is GrabbableObject grabbable) {
            this.GrabItem(enemy, grabbable);
        }

        else {
            this.UseHeldItem(enemy);
        }
    }

    public void UseSecondarySkill(HoarderBugAI enemy) {
        if (enemy.heldItem?.itemGrabbableObject is null) {
            PlayerControllerB hostPlayer = Helper.Players[0];
            enemy.watchingPlayer = hostPlayer;
            enemy.angryAtPlayer = hostPlayer;
            enemy.angryTimer = 15.0f;
            enemy.SetBehaviourState(HoardingBugState.CHASING);
            return;
        }

        if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) {
            _ = enemy.Reflect().InvokeInternalMethod("DropItemAndCallDropRPC", networkObject, false);
        }
    }

    public void UseSpecialAbility(HoarderBugAI enemy) => RoundManager.PlayRandomClip(enemy.creatureVoice, enemy.chitterSFX, true, 1.0f, 0);

    public string GetPrimarySkillName(HoarderBugAI enemy) => enemy.heldItem is not null ? "Use item" : "Grab Item";

    public string GetSecondarySkillName(HoarderBugAI enemy) => enemy.heldItem is null ? "" : "Drop item";

    public void OnOutsideStatusChange(HoarderBugAI enemy) {
        enemy.StopSearch(enemy.searchForItems, true);
        enemy.StopSearch(enemy.searchForPlayer, true);
    }
}
