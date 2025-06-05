using System.Collections.Generic;
using UnityEngine;
using ZLinq;


static partial class Helper {
    internal static HashSet<GrabbableObject> Grabbables { get; } = Helper.LocalPlayer is not null
        ? Helper.FindObjects<GrabbableObject>().WhereIsNotNull().AsValueEnumerable().Where(scrap => scrap.IsSpawned).ToHashSet()
        : [];

    internal static void ShootShotgun(this ShotgunItem item, Transform origin) {
        item.gunShootAudio.volume = 0.15f;
        item.shotgunRayPoint = origin;
        item.ShootGunAndSync(false);
    }
}
