using Hax;
using Unity.Netcode;
using Vector3 = UnityEngine.Vector3;

enum BaboonState {
    SCOUTING = 0,
    RETURNING = 1,
    AGGRESSIVE = 2,
}

internal class BaboonHawkController : IEnemyController<BaboonBirdAI> {
    Vector3 CustomCamp { get; } = new Vector3(1000.0f, 0.0f, 0.0f);
    Vector3 OriginalCamp { get; set; } = Vector3.zero;

    public void OnDeath(BaboonBirdAI enemyInstance) {
        if (enemyInstance.heldScrap is not null) {
            _ = enemyInstance.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
        }
    }

    public void OnPossess(BaboonBirdAI _) {
        if (BaboonBirdAI.baboonCampPosition != this.CustomCamp) return;

        this.OriginalCamp = BaboonBirdAI.baboonCampPosition;
        BaboonBirdAI.baboonCampPosition = this.CustomCamp;
    }

    public void OnUnpossess(BaboonBirdAI _) {
        if (BaboonBirdAI.baboonCampPosition == this.OriginalCamp) return;
        BaboonBirdAI.baboonCampPosition = this.OriginalCamp;
    }

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
    }

    public void UseSecondarySkill(BaboonBirdAI enemyInstance) {
        if (enemyInstance.heldScrap is null) return;
        _ = enemyInstance.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
    }

    public string GetPrimarySkillName(BaboonBirdAI enemyInstance) => enemyInstance.heldScrap is not null ? "" : "Grab Item";

    public string GetSecondarySkillName(BaboonBirdAI enemyInstance) => enemyInstance.heldScrap is null ? "" : "Drop item";

    public float InteractRange(BaboonBirdAI _) => 1.5f;
}
