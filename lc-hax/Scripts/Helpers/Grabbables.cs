using System.Collections.Generic;
using System.Linq;
using UnityEngine;


static partial class Helper {
    internal static HashSet<GrabbableObject> Grabbables { get; } = Helper.LocalPlayer is not null
        ? [.. Helper.FindObjects<GrabbableObject>().WhereIsNotNull().Where(scrap => scrap.IsSpawned)]
        : [];

    internal static void ShootShotgun(this ShotgunItem item, Transform origin) {
        item.gunShootAudio.volume = 0.15f;
        item.shotgunRayPoint = origin;
        item.ShootGunAndSync(false);
    }
}
