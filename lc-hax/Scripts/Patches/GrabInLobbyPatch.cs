#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Update))]
sealed class GrabInLobbyPatch {
    static void Postfix(GrabbableObject __instance) => __instance.itemProperties.canBeGrabbedBeforeGameStart = true;
}
