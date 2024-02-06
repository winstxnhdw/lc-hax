using Unity.Netcode;
using Hax;
enum Baboon {
    Scouting = 0,
    ReturningToCamp = 1,
    FocusingOnThreat = 2,
    // Additional states can be added here if identified in other parts of the code.
}

internal class BaboonHawkController : IEnemyController<BaboonBirdAI> {
    void GrabItemAndSync(BaboonBirdAI enemyInstance, GrabbableObject item) {
        if (!item.TryGetComponent(out NetworkObject netItem)) return;
        _ = enemyInstance.Reflect().InvokeInternalMethod("GrabItemAndSync", netItem);
    }

    internal void UsePrimarySkill(BaboonBirdAI enemyInstance) {
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

    internal void UseSecondarySkill(BaboonBirdAI enemyInstance) {
        if (enemyInstance.heldScrap is null) return;
        _ = enemyInstance.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
    }

    internal string GetPrimarySkillName(BaboonBirdAI enemyInstance) => enemyInstance.heldScrap is not null ? "" : "Grab Item";

    internal string GetSecondarySkillName(BaboonBirdAI enemyInstance) => enemyInstance.heldScrap is null ? "" : "Drop item";
}
