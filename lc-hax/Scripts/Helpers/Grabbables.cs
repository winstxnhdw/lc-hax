using System.Collections.Generic;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static HashSet<GrabbableObject> Grabbables { get; } = [];


    public static void InteractWithProp(this GrabbableObject item) {
        if (item == null || Helper.LocalPlayer == null) return;
        if (!item.IsOwner) {
            item.ChangeOwnershipOfProp(Helper.LocalPlayer.actualClientId);
        }
        if (item.TryGetComponent(out AnimatedItem animated)) {
            animated.EquipItem();
            return;
        }
        if (item.TryGetComponent(out NoisemakerProp noiseprop)) {
            noiseprop.ItemActivate(true, true);
            return;
        }
        if (item.TryGetComponent(out BoomboxItem BoomBox)) {
            BoomBox.ItemActivate(true, true);
            return;
        }
        if (item.TryGetComponent(out WhoopieCushionItem fartcushion)) {
            fartcushion.Fart();
            return;
        }

        item.FallToGround(false);
        item.DropSFX();

    }

    public static void DropSFX(this GrabbableObject item) {
        if (item == null) return;
        if (item.itemProperties.dropSFX != null) {
            item.gameObject.GetComponent<AudioSource>().PlayOneShot(item.itemProperties.dropSFX);
            if (item.IsOwner) {
                RoundManager.Instance.PlayAudibleNoise(item.transform.position, 8f, 0.5f, 0, item.isInElevator && StartOfRound.Instance.hangarDoorsClosed, 941);
            }
        }
    }
}
