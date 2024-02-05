using Hax;
using Unity.Netcode;

public static class BaboonController {

    public static void UsePrimarySkill(this BaboonBirdAI instance) {
        if (instance.heldScrap == null) {
            instance.GrabNearbyItem();
        }
        else {
            if (instance.heldScrap is ShotgunItem shotgun) {
                shotgun.ShootShotgun(instance.transform);
            }
            else {
                instance.heldScrap.InteractWithProp(true);
            }
        }
    }

    public static void UseSecondarySkill(this BaboonBirdAI instance) {
        if (instance.heldScrap != null) {
            instance.DropCurrentItem();
        }
    }
    public static void GrabNearbyItem(this BaboonBirdAI instance) {
        if (instance is null) return;
        if (instance.heldScrap != null) return;
        if (instance.FindNearbyItem() is not GrabbableObject item) return;
        instance.SwitchToBehaviourServerRpc(1);
        instance.GrabItemAndSync(item);
    }

    public static void GrabItemAndSync(this BaboonBirdAI instance, GrabbableObject item) {
        if (item is null) return;
        if (item.TryGetComponent(out NetworkObject netItem)) _ = instance.Reflect().InvokeInternalMethod("GrabItemAndSync", netItem);
    }

    public static void DropCurrentItem(this BaboonBirdAI instance) {
        if (instance.heldScrap != null) {
            _ = instance.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
        }
    }

    public static void OnEnemyDeath(this BaboonBirdAI instance) => instance.DropCurrentItem();
    public static string GetPrimarySkillName(this BaboonBirdAI instance) => instance == null ? "" : (instance.heldScrap != null) ? "" : "Grab Item";

    public static string GetSecondarySkillName(this BaboonBirdAI instance) => instance.heldScrap is null ? "" : "Drop item";
}
