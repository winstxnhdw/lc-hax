using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.LateUpdate))]
sealed class OneHandedItemPatch {
    static void Postfix(PlayerControllerB __instance) => __instance.twoHanded = false;
}
