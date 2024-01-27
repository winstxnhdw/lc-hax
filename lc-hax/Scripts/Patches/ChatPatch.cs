#pragma warning disable IDE1006

using System;
using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using TMPro;

[HarmonyPatch(typeof(HUDManager), "EnableChat_performed")]
class EnableChatPatch {
    static void Prefix(ref PlayerControllerB ___localPlayer, ref TMP_InputField __chatTextField, ref bool __state) {
        if (___localPlayer is not PlayerControllerB localPlayer) return;

        __chatTextField.characterLimit = int.MaxValue;
        __state = localPlayer.isPlayerDead;
        localPlayer.isPlayerDead = false;
    }

    static void Postfix(ref PlayerControllerB ___localPlayer, bool __state) => ___localPlayer.isPlayerDead = __state;
}

[HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
class SubmitChatPatch {
    static bool Prefix(ref PlayerControllerB ___localPlayer, ref bool __state) {
        __state = ___localPlayer.isPlayerDead;
        ___localPlayer.isPlayerDead = false;

        if (Helper.HUDManager is not HUDManager hudManager) {
            return true;
        }

        if (!hudManager.chatTextField.text.StartsWith("/")) {
            return true;
        }

        Helper.Try(() => Chat.ExecuteCommand(hudManager.chatTextField.text),
            (Exception exception) => Logger.Write(exception.ToString())
        );

        return false;
    }

    static void Postfix(ref PlayerControllerB ___localPlayer, bool __state) => ___localPlayer.isPlayerDead = __state;
}
