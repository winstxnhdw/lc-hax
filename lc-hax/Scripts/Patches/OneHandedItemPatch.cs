#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
class OneHandedItemPatch {
    static void Postfix(PlayerControllerB __instance) {
        if (!__instance.IsSelf()) {
            return;
        }

        __instance.twoHanded = false;
    }
}
