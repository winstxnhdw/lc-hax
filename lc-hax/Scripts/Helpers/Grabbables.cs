using System.Collections.Generic;
using UnityEngine;

namespace Hax;

public static partial class Helper {
    public static HashSet<GrabbableObject> Grabbables { get; } = [];

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
