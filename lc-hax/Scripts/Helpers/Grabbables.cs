using System.Collections.Generic;
using System.Linq;

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
}
