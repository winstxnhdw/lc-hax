using Hax;
using Unity.Netcode;
using Vector3 = UnityEngine.Vector3;


internal enum BaboonState
{
    SCOUTING = 0,
    RETURNING = 1,
    AGGRESSIVE = 2
}

internal class BaboonHawkController : IEnemyController<BaboonBirdAI>
{
    private Vector3 OriginalCamp { get; set; } = Vector3.zero;
    private Vector3 CustomCamp { get; } = new(1000.0f, 1000.0f, 1000.0f);

    public void OnDeath(BaboonBirdAI enemy)
    {
        if (enemy.heldScrap is not null) _ = enemy.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
    }


    public void OnOutsideStatusChange(BaboonBirdAI enemy)
    {
        enemy.StopSearch(enemy.scoutingSearchRoutine, true);
    }


    public void OnPossess(BaboonBirdAI _)
    {
        if (BaboonBirdAI.baboonCampPosition != CustomCamp) return;

        OriginalCamp = BaboonBirdAI.baboonCampPosition;
        BaboonBirdAI.baboonCampPosition = CustomCamp;
    }

    public void OnUnpossess(BaboonBirdAI _)
    {
        if (BaboonBirdAI.baboonCampPosition == OriginalCamp) return;
        BaboonBirdAI.baboonCampPosition = OriginalCamp;
    }

    public void UsePrimarySkill(BaboonBirdAI enemy)
    {
        if (enemy.heldScrap is null && enemy.FindNearbyItem(1.5f) is GrabbableObject grabbable)
        {
            GrabItemAndSync(enemy, grabbable);
            return;
        }

        if (enemy.heldScrap is ShotgunItem shotgun)
        {
            shotgun.ShootShotgun(enemy.transform);
            return;
        }

        enemy.heldScrap?.InteractWithProp();
    }

    public void UseSecondarySkill(BaboonBirdAI enemy)
    {
        if (enemy.heldScrap is null) return;
        _ = enemy.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
    }

    public string GetPrimarySkillName(BaboonBirdAI enemy)
    {
        return enemy.heldScrap is not null ? "" : "Grab Item";
    }

    public string GetSecondarySkillName(BaboonBirdAI enemy)
    {
        return enemy.heldScrap is null ? "" : "Drop item";
    }

    private void EnableAIControl(BaboonBirdAI enemy, bool enabled)
    {
        if (enabled)
            BaboonBirdAI.baboonCampPosition = OriginalCamp;
        else
            BaboonBirdAI.baboonCampPosition = CustomCamp;
    }

    private void GrabItemAndSync(BaboonBirdAI enemy, GrabbableObject item)
    {
        if (!item.TryGetComponent(out NetworkObject netItem)) return;
        enemy.SetBehaviourState(BaboonState.RETURNING);
        _ = enemy.Reflect().InvokeInternalMethod("GrabItemAndSync", netItem);
    }
}