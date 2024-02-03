using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static HashSet<GrabbableObject> Grabbables { get; } = [];

    static void DropSFX(this GrabbableObject item) {
        if (!item.gameObject.TryGetComponent(out AudioSource audio)) return;

        audio.PlayOneShot(item.itemProperties.dropSFX);

        Helper.RoundManager?.PlayAudibleNoise(
            noisePosition: item.transform.position,
            noiseRange: 8.0f,
            noiseLoudness: 0.5f,
            timesPlayedInSameSpot: 0,
            noiseIsInsideClosedShip: item.isInElevator && Helper.StartOfRound?.hangarDoorsClosed is true,
            noiseID: 941
        );
    }

    public static void InteractWithProp(this GrabbableObject item, bool playSFX = false) {
        if (Helper.LocalPlayer is PlayerControllerB localPlayer && !item.IsOwner) {
            item.ChangeOwnershipOfProp(localPlayer.actualClientId);
        }

        if (playSFX) {
            item.DropSFX();
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

        item.UseItemOnClient(true);
    }

    public static void ShootShotgun(this ShotgunItem item, Transform origin) {
        item.gunShootAudio.volume = 0.15f;
        item.shotgunRayPoint = origin;
        item.ShootGunAndSync(false);
    }
}
