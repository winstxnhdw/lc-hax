#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Start))]
class ItemsPatch {
    static void Postfix(Item? ___itemProperties) {
        if (___itemProperties == null) return;
        ___itemProperties.canBeGrabbedBeforeGameStart = true;
        ___itemProperties.twoHanded = false;
    }
}
