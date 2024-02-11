#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
class OneHandedItemPatch {
    static void Postfix(PlayerControllerB __instance) => __instance.twoHanded = false;
}
