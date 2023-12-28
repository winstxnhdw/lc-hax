#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB), "SetHoverTipAndCurrentInteractTrigger")]
class NameBillboardPatch {
    static void Prefix(ref PlayerControllerB __instance) {
        __instance.ShowNameBillboard();
    }
}
