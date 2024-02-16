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


    public void OnMovement(HoarderBugAI enemyInstance, bool isMoving, bool isSprinting) {
        if (enemyInstance.heldItem == null) return;
        if (enemyInstance.heldItem.itemGrabbableObject == null) return;
        if (enemyInstance.heldItem.itemGrabbableObject) {
            enemyInstance.angryTimer = 0f;
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

    public void OnDeath(HoarderBugAI enemyInstance) {
        if (enemyInstance.heldItem.itemGrabbableObject is not GrabbableObject grabbable) return;
        if (!grabbable.TryGetComponent(out NetworkObject networkObject)) return;

        _ = enemyInstance.Reflect().InvokeInternalMethod(
            "DropItemAndCallDropRPC",
            networkObject,
            false
        );
    }

    void UseHeldItem(HoarderBugAI enemyInstance) {
        if (enemyInstance.heldItem?.itemGrabbableObject != null) {
            switch (enemyInstance.heldItem.itemGrabbableObject) {
                case ShotgunItem gun:
                    gun.ShootShotgun(enemyInstance.transform);
                    break;

                default:
                    enemyInstance.heldItem.itemGrabbableObject?.InteractWithProp();
                    break;
            }
        }
    }


    public void UsePrimarySkill(HoarderBugAI enemyInstance) {
        if (enemyInstance.angryTimer > 0f) {
            enemyInstance.angryTimer = 0f;
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
        if (enemyInstance.heldItem == null || enemyInstance.heldItem.itemGrabbableObject == null) {
            if (enemyInstance.angryTimer <= 0f) {
                enemyInstance.watchingPlayer = Helper.LocalPlayer;
                enemyInstance.angryAtPlayer = Helper.LocalPlayer; // anger it to self just to trigger it.
                enemyInstance.angryTimer = 15f;
                enemyInstance.SetBehaviourState(HoarderBugState.CHASING_PLAYER);
            }
        }
        else {
            if (enemyInstance.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) {
                _  = enemyInstance.Reflect().InvokeInternalMethod(
                    "DropItemAndCallDropRPC",
                    networkObject,
                    false
                );

            }
        }
    }

    public string GetPrimarySkillName(HoarderBugAI enemyInstance) => enemyInstance.heldItem is not null ? "Use item" : "Grab Item";

    public string GetSecondarySkillName(HoarderBugAI enemyInstance) => enemyInstance.heldItem is null ? "" : "Drop item";

    public float? InteractRange(HoarderBugAI _) => 1f;

}
