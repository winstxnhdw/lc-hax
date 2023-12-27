#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB))]
[HarmonyPatch("LateUpdate")]
class InfiniteGrabPatch {
    static void Postfix(ref float ___grabDistance) {
        ___grabDistance = float.MaxValue;
    }
}
