#pragma warning disable IDE1006

using System;
using System.Linq;
using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(HUDManager), "EnableChat_performed")]
class EnableChatPatch {
    static void Prefix(HUDManager __instance, ref bool __state) {
        if (__instance.localPlayer is not PlayerControllerB localPlayer) return;

        __instance.chatTextField.characterLimit = int.MaxValue;
        __state = localPlayer.isPlayerDead;
        localPlayer.isPlayerDead = false;
    }

    static void Postfix(ref PlayerControllerB ___localPlayer, bool __state) => ___localPlayer.isPlayerDead = __state;
}

[HarmonyBefore]
[HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
class SubmitChatPatch {
    static bool Prefix(HUDManager __instance, ref bool __state) {
        __state = __instance.localPlayer.isPlayerDead;
        __instance.localPlayer.isPlayerDead = false;

        if (Helper.HUDManager is not HUDManager hudManager) {
            return true;
        }

        if (!new[] { '!', State.CommandPrefix }.Any(hudManager.chatTextField.text.StartsWith)) {
            return true;
        }

        try {
            Chat.ExecuteCommand(hudManager.chatTextField.text.TrimEnd());
        }

        catch (Exception exception) {
            Logger.Write(exception.ToString());
        }

        return false;
    }

    static void Postfix(HUDManager __instance, bool __state) => __instance.localPlayer.isPlayerDead = __state;
}
