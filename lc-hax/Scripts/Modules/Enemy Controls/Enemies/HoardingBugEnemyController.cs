using Hax;
using System;
using Unity.Netcode;
using UnityEngine;

public static class HoardingBugController {
    public static void UsePrimarySkill(this HoarderBugAI instance) {
        if (instance == null) return;
        if (instance.heldItem != null) {
            instance.UseHeldItem();
        }
    }

    public static void UseSecondarySkill(this HoarderBugAI instance) {
        if (instance == null) return;
        if (instance.heldItem == null) {
            instance.GrabNearbyItem();
        }
        else {
            instance.DropCurrentItem();
        }
    }

    public static void UseHeldItem(this HoarderBugAI instance) {
        if (instance == null) return;
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
        if (instance == null) return;
        if (instance.heldItem == null) return;
        GrabbableObject? item = instance.FindNearbyItem();
        if (item != null) instance.GrabTargetItemIfClose(item);
    }

    public static GrabbableObject? FindNearbyItem(this HoarderBugAI instance, float grabRange = 1.5f) {
        if (instance == null) return null;
        if (instance.heldItem == null) {
            Collider[] Search = Physics.OverlapSphere(instance.gameObject.transform.position, grabRange);
            for (int i = 0; i < Search.Length; i++) {
                if (!Search[i].TryGetComponent(out GrabbableObject item)) continue;
                if (item.TryGetComponent(out NetworkObject network)) {
                    return item;
                }
            }
        }

        return null;
    }

    public static void GrabTargetItemIfClose(this HoarderBugAI instance, GrabbableObject item) {
        if (instance == null) return;
        if (instance.heldItem != null) return;
        if (instance.targetItem == null) return;
        instance.targetItem = item;
        _ = instance.Reflect().InvokeInternalMethod("GrabTargetItemIfClose");
    }

    public static void DropCurrentItem(this HoarderBugAI instance) {
        if (instance == null) return;
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
        return instance == null ? "" : (instance.heldItem == null) ? "Grab item" : "Drop item";
    }
}