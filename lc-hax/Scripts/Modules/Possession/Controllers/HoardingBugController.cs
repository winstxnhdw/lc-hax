using GameNetcodeStuff;
using Hax;
using Unity.Netcode;

public enum HoarderBugState {
    IDLE,
    SEARCHING_FOR_ITEMS,
    RETURNING_TO_NEST,
    CHASING_PLAYER,
    WATCHING_PLAYER,
    AT_NEST
}
internal class HoardingBugController : IEnemyController<HoarderBugAI> {
    void UseHeldItem(HoarderBugAI enemyInstance) {
        if (enemyInstance.heldItem?.itemGrabbableObject is not GrabbableObject grabbable) return;

        switch (grabbable) {
            case ShotgunItem gun:
                gun.ShootShotgun(enemyInstance.transform);
                break;

            default:
                break;
        }
    }

    void GrabItem(HoarderBugAI enemyInstance, GrabbableObject item) {
        if (!item.TryGetComponent(out NetworkObject netItem)) return;

        _ = enemyInstance.Reflect()
                         .InvokeInternalMethod("GrabItem", netItem)?
                         .SetInternalField("sendingGrabOrDropRPC", true);

        enemyInstance.SwitchToBehaviourServerRpc(1);
        enemyInstance.GrabItemServerRpc(netItem);
    }

    public void OnMovement(HoarderBugAI enemyInstance, bool isMoving, bool isSprinting) {
        if (enemyInstance.heldItem?.itemGrabbableObject is null) return;
        enemyInstance.angryTimer = 0.0f;
    }

    public void OnDeath(HoarderBugAI enemyInstance) {
        if (enemyInstance.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) return;

        _ = enemyInstance.Reflect().InvokeInternalMethod(
            "DropItemAndCallDropRPC",
            networkObject,
            false
        );
    }

    public void UsePrimarySkill(HoarderBugAI enemyInstance) {
        if (enemyInstance.angryTimer > 0.0f) {
            enemyInstance.angryTimer = 0.0f;
            enemyInstance.angryAtPlayer = null;
        }

        if (enemyInstance.heldItem is null && enemyInstance.FindNearbyItem() is GrabbableObject grabbable) {
            this.GrabItem(enemyInstance, grabbable);
        }

        else {
            this.UseHeldItem(enemyInstance);
        }
    }

    public void UseSecondarySkill(HoarderBugAI enemyInstance) {
        if (enemyInstance.heldItem?.itemGrabbableObject is null) {
            PlayerControllerB hostPlayer = Helper.Players[0];
            enemyInstance.watchingPlayer = hostPlayer;
            enemyInstance.angryAtPlayer = hostPlayer;
            enemyInstance.angryTimer = 15.0f;
            enemyInstance.SetBehaviourState(HoarderBugState.CHASING_PLAYER);
            return;
        }

        if (enemyInstance.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) {
            _ = enemyInstance.Reflect().InvokeInternalMethod("DropItemAndCallDropRPC", networkObject, false);
        }
    }

    public string GetPrimarySkillName(HoarderBugAI enemyInstance) => enemyInstance.heldItem is not null ? "Use item" : "Grab Item";

    public string GetSecondarySkillName(HoarderBugAI enemyInstance) => enemyInstance.heldItem is null ? "" : "Drop item";

    public float InteractRange(HoarderBugAI _) => 1.0f;
}
