#pragma warning disable IDE1006

#region

using System;
using System.Linq;
using HarmonyLib;
using GameNetcodeStuff;
using Hax;

#endregion

[HarmonyPatch]
class ChatPatches {
    static bool isLocalPlayerDeadStatus;
    static bool isLocalPlayerDeadStatus2;

    [HarmonyBefore]
    [HarmonyPatch(typeof(HUDManager), "EnableChat_performed")]
    [HarmonyPrefix]
    static void EnableChatPrefix(HUDManager __instance) {
        if (__instance.localPlayer is not PlayerControllerB localPlayer) return;
        if (!localPlayer.IsSelf()) return;

        isLocalPlayerDeadStatus = localPlayer.isPlayerDead;
        localPlayer.isPlayerDead = false;
    }

    [HarmonyPatch(typeof(HUDManager), "EnableChat_performed")]
    [HarmonyPostfix]
    static void EnableChatPostfix(HUDManager __instance) {
        if (__instance.localPlayer is not PlayerControllerB localPlayer) return;
        if (!localPlayer.IsSelf()) return;
        localPlayer.isPlayerDead = isLocalPlayerDeadStatus;
    }

    [HarmonyBefore]
    [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
    [HarmonyPrefix]
    static bool SubmitChatPrefix(HUDManager __instance) {
        if (__instance is not HUDManager hudManager) return true;
        if (__instance.localPlayer is not PlayerControllerB localPlayer) return true;
        if (!localPlayer.IsSelf()) return true;
        isLocalPlayerDeadStatus2 = localPlayer.isPlayerDead;
        localPlayer.isPlayerDead = false;


        if (!new[] { '!', State.CommandPrefix }.Any(hudManager.chatTextField.text.StartsWith)) return true;

        try {
            Chat.ExecuteCommand(hudManager.chatTextField.text.TrimEnd());
        }
        catch (Exception exception) {
            Logger.Write(exception.ToString());
        }

        return false;
    }

    [HarmonyPatch(typeof(HUDManager), "SubmitChat_performed")]
    [HarmonyPostfix]
    static void SubmitChatPostfix(HUDManager __instance) {
        if (__instance is not HUDManager hudManager) return;
        if (hudManager.localPlayer is not PlayerControllerB localPlayer) return;
        if (!localPlayer.IsSelf()) return;
        localPlayer.isPlayerDead = isLocalPlayerDeadStatus2;
    }
}
