#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.LateUpdate))]
class OneHandedItemPatch {
    static void Postfix(ref Item ___itemProperties) {
        ___itemProperties.twoHanded = false;
    }
}
