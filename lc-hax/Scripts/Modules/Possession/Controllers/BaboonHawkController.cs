using Hax;
using Unity.Netcode;
using Vector3 = UnityEngine.Vector3;

enum BaboonState {
    SCOUTING = 0,
    RETURNING = 1,
    AGGRESSIVE = 2,
}

class BaboonHawkController : IEnemyController<BaboonBirdAI> {
    Vector3 CustomCamp { get; } = new Vector3(1000.0f, 0.0f, 0.0f);
    Vector3 OriginalCamp { get; set; } = Vector3.zero;

    public void OnDeath(BaboonBirdAI enemy) {
        if (enemy.heldScrap is not null) {
            _ = enemy.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
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

    void GrabItemAndSync(BaboonBirdAI enemy, GrabbableObject item) {
        if (!item.TryGetComponent(out NetworkObject netItem)) return;
        _ = enemy.Reflect().InvokeInternalMethod("GrabItemAndSync", netItem);
    }

    public void UsePrimarySkill(BaboonBirdAI enemy) {
        if (enemy.heldScrap is null && enemy.FindNearbyItem() is GrabbableObject grabbable) {
            this.GrabItemAndSync(enemy, grabbable);
            return;
        }

        if (enemy.heldScrap is ShotgunItem shotgun) {
            shotgun.ShootShotgun(enemy.transform);
            return;
        }
    }

    public void UseSecondarySkill(BaboonBirdAI enemy) {
        if (enemy.heldScrap is null) return;
        _ = enemy.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
    }

    public string GetPrimarySkillName(BaboonBirdAI enemy) => enemy.heldScrap is not null ? "" : "Grab Item";

    public string GetSecondarySkillName(BaboonBirdAI enemy) => enemy.heldScrap is null ? "" : "Drop item";

    public float InteractRange(BaboonBirdAI _) => 1.5f;
}
