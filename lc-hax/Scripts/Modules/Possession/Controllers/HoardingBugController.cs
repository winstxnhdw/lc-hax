using Hax;
using Unity.Netcode;

internal class HoardingBugController : IEnemyController<HoarderBugAI> {
    void GrabItem(HoarderBugAI enemyInstance, GrabbableObject item) {
        if (!item.TryGetComponent(out NetworkObject netItem)) return;

        _ = enemyInstance.Reflect()
                         .InvokeInternalMethod("GrabItem", netItem)?
                         .SetInternalField("sendingGrabOrDropRPC", true);

        enemyInstance.SwitchToBehaviourServerRpc(1);
        enemyInstance.GrabItemServerRpc(netItem);
    }

    void UseHeldItem(HoarderBugAI enemyInstance) {
        switch (enemyInstance.heldItem.itemGrabbableObject) {
            case ShotgunItem gun:
                gun.ShootShotgun(enemyInstance.transform);
                break;

            default:
                enemyInstance.heldItem.itemGrabbableObject.InteractWithProp(true);
                break;
        }
    }

    public void UsePrimarySkill(HoarderBugAI enemyInstance) {
        if (enemyInstance.heldItem is null && enemyInstance.FindNearbyItem() is GrabbableObject grabbable) {
            this.GrabItem(enemyInstance, grabbable);
        }

        else {
            this.UseHeldItem(enemyInstance);
        }
    }

    public void UseSecondarySkill(HoarderBugAI enemyInstance) {
        if (enemyInstance.heldItem.itemGrabbableObject is not GrabbableObject grabbable) return;
        if (!grabbable.TryGetComponent(out NetworkObject networkObject)) return;

        _ = enemyInstance.Reflect().InvokeInternalMethod(
            "DropItemAndCallDropRPC",
            networkObject,
            false
        );
    }

    public CharArray GetPrimarySkillName(HoarderBugAI enemyInstance) => enemyInstance.heldItem is not null ? "Use item" : "Grab Item";

    public CharArray GetSecondarySkillName(HoarderBugAI enemyInstance) => enemyInstance.heldItem is null ? "" : "Drop item";
}
