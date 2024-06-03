#pragma warning disable IDE1006

#region

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

#endregion

[HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
class OneHandedItemPatch {
    static void Postfix(PlayerControllerB __instance) {
        if (!__instance.IsSelf()) return;

        __instance.twoHanded = false;
    }
}
