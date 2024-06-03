#pragma warning disable IDE1006

#region

using HarmonyLib;

#endregion

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Update))]
class GrabInLobbyPatch {
    static void Postfix(GrabbableObject __instance) => __instance.itemProperties.canBeGrabbedBeforeGameStart = true;
}
