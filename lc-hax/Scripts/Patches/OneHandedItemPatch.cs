#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
sealed class OneHandedItemPatch {
    static void Postfix(PlayerControllerB __instance) => __instance.twoHanded = false;
}
