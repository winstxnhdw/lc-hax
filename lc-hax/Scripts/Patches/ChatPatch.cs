#pragma warning disable IDE1006

using SystemException = System.Exception;
using GameNetcodeStuff;
using HarmonyLib;
using Hax;

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
    static bool Prefix(ref PlayerControllerB ___localPlayer, ref bool __state) {
        __state = ___localPlayer.isPlayerDead;
        ___localPlayer.isPlayerDead = false;

        if (!Helper.HUDManager.IsNotNull(out HUDManager hudManager)) {
            return true;
        }

        if (!hudManager.chatTextField.text.StartsWith("/")) {
            return true;
        }

        Helper.Try(() => Chat.ExecuteCommand(hudManager.chatTextField.text),
            (SystemException exception) => Logger.Write(exception.ToString())
        );

        return false;
    }

    static void Postfix(ref PlayerControllerB ___localPlayer, bool __state) => ___localPlayer.isPlayerDead = __state;
}
