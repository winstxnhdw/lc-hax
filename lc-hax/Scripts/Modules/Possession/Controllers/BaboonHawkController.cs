using System.Numerics;
using Hax;
using Unity.Netcode;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

enum BaboonState {
    SCOUTING = 0,
    RETURNING = 1,
    AGGRESSIVE = 2,
}

internal class BaboonHawkController : IEnemyController<BaboonBirdAI> {

    Vector3 customCamp = new(1000, 1000, 1000);

    Vector3 originalCamp = new(0, 0, 0);
    public bool CanUseEntranceDoors(BaboonBirdAI _) => true;

    public void OnDeath(BaboonBirdAI enemyInstance) {
        if (enemyInstance.heldScrap is not null) {
            _ = enemyInstance.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
        }
    }

    public void OnPossess(BaboonBirdAI enemyInstance) {
        if (BaboonBirdAI.baboonCampPosition != this.customCamp) return;
        this.originalCamp = BaboonBirdAI.baboonCampPosition;
        BaboonBirdAI.baboonCampPosition = this.customCamp;
    }

    public void OnUnpossess(BaboonBirdAI enemyInstance) {
        if (BaboonBirdAI.baboonCampPosition != this.customCamp) {
            BaboonBirdAI.baboonCampPosition = this.originalCamp;
        }
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

        enemyInstance.heldScrap?.InteractWithProp();
    }

    public void UseSecondarySkill(BaboonBirdAI enemyInstance) {
        if (enemyInstance.heldScrap is null) return;
        _ = enemyInstance.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
    }

    public string GetPrimarySkillName(BaboonBirdAI enemyInstance) => enemyInstance.heldScrap is not null ? "" : "Grab Item";

    public string GetSecondarySkillName(BaboonBirdAI enemyInstance) => enemyInstance.heldScrap is null ? "" : "Drop item";
}
