using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

internal static partial class Helper {
    internal static HashSet<GrabbableObject> Grabbables { get; } = Helper.LocalPlayer is null ? [] :
        Helper.FindObjects<GrabbableObject>()
              .WhereIsNotNull()
              .Where(scrap => scrap.IsSpawned)
              .ToHashSet();


    internal static void InteractWithProp(this GrabbableObject grabbable) {
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


        grabbable.UseItemOnClient(true);
    }

    internal static void ShootShotgun(this ShotgunItem item, Transform origin) {
        item.gunShootAudio.volume = 0.15f;
        item.shotgunRayPoint = origin;
        item.ShootGunAndSync(false);
    }

    internal static GrabbableObject? GetGrabbableFromGift(this GiftBoxItem giftBox) {
        GameObject? content = giftBox.Reflect().GetInternalField<GameObject>("objectInPresent");
        if(content == null) return null;
        if(content.TryGetComponent(out GrabbableObject grabbable)) {
            return grabbable;
        }
        return null;
    }


    internal static string ToEspLabel(this GrabbableObject grabbable) {
        if(grabbable == null) return "";
        if (grabbable is RagdollGrabbableObject ragdollGrabbableObject) {
            PlayerControllerB? player = ragdollGrabbableObject.GetPlayerFromBody();
            return player == null ? "Body" : $"Body of {player.playerUsername}";
        }
        else if (grabbable is GiftBoxItem giftBox) {
            GrabbableObject? content = giftBox.GetGrabbableFromGift();
            if (content != null) {
                return $"Gift : ({content.itemProperties.itemName})";
            }
        }
        return grabbable.itemProperties.itemName;
    }
}
