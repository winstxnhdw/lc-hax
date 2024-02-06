using Unity.Netcode;
using Hax;

enum BaboonState {
    SCOUTING = 0,
    RETURNING = 1,
    AGGRESSIVE = 2,
}

internal class BaboonHawkController : IEnemyController<BaboonBirdAI> {
    void GrabItemAndSync(BaboonBirdAI enemyInstance, GrabbableObject item) {
        if (!item.TryGetComponent(out NetworkObject netItem)) return;
        _ = enemyInstance.Reflect().InvokeInternalMethod("GrabItemAndSync", netItem);
    }

    public void UsePrimarySkill(BaboonBirdAI enemyInstance) {
        if (enemyInstance.heldScrap is null && enemyInstance.FindNearbyItem() is GrabbableObject grabbable) {
            this.GrabItemAndSync(enemyInstance, grabbable);
            return;
        }

        if (enemyInstance.heldScrap is ShotgunItem shotgun) {
            shotgun.ShootShotgun(enemyInstance.transform);
            return;
        }

        enemyInstance.heldScrap?.InteractWithProp(true);
    }

    public void UseSecondarySkill(BaboonBirdAI enemyInstance) {
        if (enemyInstance.heldScrap is null) return;
        _ = enemyInstance.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
    }

    public CharArray GetPrimarySkillName(BaboonBirdAI enemyInstance) => enemyInstance.heldScrap is not null ? "" : "Grab Item";

    public CharArray GetSecondarySkillName(BaboonBirdAI enemyInstance) => enemyInstance.heldScrap is null ? "" : "Drop item";
}
