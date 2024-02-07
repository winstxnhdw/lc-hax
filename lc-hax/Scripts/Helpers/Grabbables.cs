using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

internal static partial class Helper {
    internal static HashSet<GrabbableObject> Grabbables { get; } = [];

    static void PlayDropSFX(this GrabbableObject item) {
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

    internal static void InteractWithProp(this GrabbableObject grabbable, bool playSFX = false) {
        if (Helper.LocalPlayer is PlayerControllerB localPlayer && !grabbable.IsOwner) {
            grabbable.ChangeOwnershipOfProp(localPlayer.actualClientId);
        }

        if (grabbable.TryGetComponent(out AnimatedItem animatedItem)) {
            animatedItem.EquipItem();
            return;
        }

        if (grabbable.TryGetComponent(out NoisemakerProp noisemakerProp)) {
            noisemakerProp.ItemActivate(true, true);
            return;
        }

        if (grabbable.TryGetComponent(out BoomboxItem boomboxItem)) {
            boomboxItem.ItemActivate(true, true);
            return;
        }

        if (grabbable.TryGetComponent(out WhoopieCushionItem whoopieCushionItem)) {
            whoopieCushionItem.Fart();
            return;
        }

        if (playSFX) {
            grabbable.PlayDropSFX();
        }

        grabbable.UseItemOnClient(true);
    }

    internal static void ShootShotgun(this ShotgunItem item, Transform origin) {
        item.gunShootAudio.volume = 0.15f;
        item.shotgunRayPoint = origin;
        item.ShootGunAndSync(false);
    }
}
