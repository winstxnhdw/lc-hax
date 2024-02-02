using Unity.Netcode;
using UnityEngine;

public static class BaboonController {


    public static void UseSecondarySkill(this BaboonBirdAI instance) {
        if (instance is null) return;
        if (instance.heldScrap == null) {
            instance.GrabNearbyItem();
        }
        else {
            instance.DropCurrentItem();
        }
    }

    public static NetworkObject? FindNearbyItem(this BaboonBirdAI instance, float range = 1.5f) {
        if (instance is null) return null;
        Collider[] Search = Physics.OverlapSphere(instance.transform.position, range);
        for (int i = 0; i < Search.Length; i++) {
            if (!Search[i].TryGetComponent(out GrabbableObject item)) continue;
            if (instance.CanGrabScrap(item))
                if (item.TryGetComponent(out NetworkObject network))
                    if (Vector3.Distance(item.transform.position, instance.transform.position) < float.PositiveInfinity) return network;
        }
        return null;
    }

    public static void GrabNearbyItem(this BaboonBirdAI instance) {
        if (instance is null) return;
        if (instance.heldScrap != null) return;
        NetworkObject? item = instance.FindNearbyItem();
        if (item != null) {
            instance.SwitchToBehaviourState(1);
            instance.GrabItemAndSync(item);
        }
    }

    public static bool CanGrabScrap(this BaboonBirdAI instance, GrabbableObject item) {
        return instance is null ? false : item is not null && instance.Reflect().InvokeInternalMethod<bool>("CanGrabScrap", item);
    }

    public static void GrabItemAndSync(this BaboonBirdAI instance, NetworkObject item) {
        instance.Reflect().InvokeInternalMethod("GrabItemAndSync", item);
    }

    public static void DropCurrentItem(this BaboonBirdAI instance) {
        if (instance.heldScrap != null) {
            instance.Reflect().InvokeInternalMethod("DropHeldItemAndSync");
        }
    }

    public static void OnEnemyDeath(this BaboonBirdAI instance) {
        instance.DropCurrentItem();
    }

    public static string GetSecondarySkillName(this BaboonBirdAI instance) {
        return (instance.heldScrap == null) ? "Grab item" : "Drop item";
    }
}
