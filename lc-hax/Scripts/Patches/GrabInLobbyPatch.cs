#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Update))]
class GrabInLobbyPatch {
    static void Postfix(Item ___itemProperties) => ___itemProperties.canBeGrabbedBeforeGameStart = true;
}
