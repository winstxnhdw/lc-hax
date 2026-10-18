using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SetHoverTipAndCurrentInteractTrigger))]
sealed class InfiniteGrabPatch {
    static void Postfix(PlayerControllerB __instance) => __instance.grabDistance = float.MaxValue;
}
