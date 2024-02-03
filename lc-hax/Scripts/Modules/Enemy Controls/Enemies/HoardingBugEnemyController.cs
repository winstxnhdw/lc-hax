using Hax;
using System;
using Unity.Netcode;

public static class HoardingBugController {
    public static void UsePrimarySkill(this HoarderBugAI instance) {
        if (instance.heldItem is null) return;
        instance.UseHeldItem();
    }

    public static void UseSecondarySkill(this HoarderBugAI instance) {
        if (instance.heldItem == null) {
            instance.GrabNearbyItem();
        }
        else {
            instance.DropCurrentItem();
        }
    }

    public static void UseHeldItem(this HoarderBugAI instance) {
        if (instance.heldItem == null) return;
        GrabbableObject itemGrabbableObject = instance.heldItem.itemGrabbableObject;
        if (itemGrabbableObject == null) return;
        Type type = itemGrabbableObject.GetType();
        if (type == null) return;
        if (type == typeof(ShotgunItem)) {
            ShotgunItem gun = (ShotgunItem)itemGrabbableObject;
            if (gun == null) return;
            gun.ShootShotgun(instance.transform);
        }
        else
            itemGrabbableObject.InteractWithProp(true);
    }

    public static void GrabNearbyItem(this HoarderBugAI instance) {
        if (instance.heldItem == null) return;
        GrabbableObject? item = instance.FindNearbyItem();
        if (item != null) {
            instance.GrabItem(item);
        }
    }

    public static void GrabItem(this HoarderBugAI instance, GrabbableObject item) {
        if (item == null) return;
        if (instance.heldItem != null) return;
        if (item.TryGetComponent(out NetworkObject netItem)) {
            instance.SwitchToBehaviourServerRpc(1);
            _ = instance.Reflect().InvokeInternalMethod("GrabItem", netItem);
            _ = instance.Reflect().SetInternalField("sendingGrabOrDropRPC", true);
            instance.GrabItemServerRpc(netItem);
        }
    }

    public static void GrabTargetItemIfClose(this HoarderBugAI instance, GrabbableObject item) {
        if (instance.heldItem != null) return;
        instance.targetItem = item;
        _ = instance.Reflect().InvokeInternalMethod("GrabTargetItemIfClose");
    }

    public static void DropCurrentItem(this HoarderBugAI instance) {
        if (instance.heldItem != null)
            _ = instance.Reflect().InvokeInternalMethod("DropItemAndCallDropRPC",
            [
                instance.heldItem.itemGrabbableObject.GetComponent<NetworkObject>(),
                false
            ]);
    }

    public static string GetPrimarySkillName(this HoarderBugAI instance) {
        return instance == null ? "" : (instance.heldItem != null) ? "Use item" : "";
    }

    public static string GetSecondarySkillName(this HoarderBugAI instance) {
        return instance.heldItem == null ? "Grab item" : "Drop item";
    }
}
