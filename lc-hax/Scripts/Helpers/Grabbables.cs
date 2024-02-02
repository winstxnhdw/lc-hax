using System.Collections.Generic;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static HashSet<GrabbableObject> Grabbables { get; } = [];

    public static void InteractWithProp(this GrabbableObject item, bool playSFX = false) {
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
        if (playSFX) item.DropSFX();
        item.UseItemOnClient(true);
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

    public static void ShootShotgun(this ShotgunItem item, Transform origin) {
        if (item == null || origin == null) return;
        item.gunShootAudio.volume = 0.15f;
        item.shotgunRayPoint = origin;
        item.ShootGunAndSync(false);

    }
}
