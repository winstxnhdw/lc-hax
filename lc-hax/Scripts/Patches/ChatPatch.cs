#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), "EnableChat_performed")]
class EnableChatPatch {
    static void Prefix(ref PlayerControllerB ___localPlayer, ref bool __state) {
        if (!___localPlayer.IsNotNull(out PlayerControllerB localPlayer)) return;

        __state = localPlayer.isPlayerDead;
        localPlayer.isPlayerDead = false;
    }

    static void Postfix(ref PlayerControllerB ___localPlayer, bool __state) => ___localPlayer.isPlayerDead = __state;
}

[HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
class SubmitChatPatch {
    static void Prefix(ref PlayerControllerB ___localPlayer, ref bool __state) {
        __state = ___localPlayer.isPlayerDead;
        ___localPlayer.isPlayerDead = false;
    }

    static void Postfix(ref PlayerControllerB ___localPlayer, bool __state) => ___localPlayer.isPlayerDead = __state;
}
