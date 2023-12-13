#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager))]
[HarmonyPatch("EnableChat_performed")]
class EnableChatPatch {
    static void Prefix(ref PlayerControllerB ___localPlayer, ref bool __state) {
        __state = ___localPlayer.isPlayerDead;
        ___localPlayer.isPlayerDead = false;
    }

    static void Postfix(ref PlayerControllerB ___localPlayer, bool __state) {
        ___localPlayer.isPlayerDead = __state;
    }
}

[HarmonyPatch(typeof(HUDManager))]
[HarmonyPatch("SubmitChat_performed")]
class SubmitChatPatch {
    static void Prefix(ref PlayerControllerB ___localPlayer, ref bool __state) {
        __state = ___localPlayer.isPlayerDead;
        ___localPlayer.isPlayerDead = false;
    }

    static void Postfix(ref PlayerControllerB ___localPlayer, bool __state) {
        ___localPlayer.isPlayerDead = __state;
    }
}
