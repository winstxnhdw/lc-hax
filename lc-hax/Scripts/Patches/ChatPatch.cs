#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager))]
class ChatPatch {
    [HarmonyPrefix]
    [HarmonyPatch("EnableChat_performed")]
    static void EnableChatPrefix(ref PlayerControllerB ___localPlayer, ref bool __state) {
        __state = ___localPlayer.isPlayerDead;
        ___localPlayer.isPlayerDead = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch("EnableChat_performed")]
    static void EnableChatPostfix(ref PlayerControllerB ___localPlayer, bool __state) {
        ___localPlayer.isPlayerDead = __state;
    }

    [HarmonyPrefix]
    [HarmonyPatch("SubmitChat_performed")]
    static void SubmitChatPrefix(ref PlayerControllerB ___localPlayer, ref bool __state) {
        __state = ___localPlayer.isPlayerDead;
        ___localPlayer.isPlayerDead = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch("SubmitChat_performed")]
    static void SubmitChatPostfix(ref PlayerControllerB ___localPlayer, bool __state) {
        ___localPlayer.isPlayerDead = __state;
    }
}
