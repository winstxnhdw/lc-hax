using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

[HarmonyPatch]
class GodModePatch {
    private static void Unfreeze() {
        if (Helper.LocalPlayer != null) {
            Helper.LocalPlayer.inAnimationWithEnemy = null;
            Helper.LocalPlayer.inSpecialInteractAnimation = false;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath))]
    static bool AllowPlayerDeathPrefix(PlayerControllerB __instance, ref bool __result) {
        if (!Setting.EnableGodMode || !__instance.IsSelf()) return true;
        __result = false;
        return false;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationServerRpc))]
    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationClientRpc))]
    static bool PrefixFlowermanKill(int playerObjectId) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerObjectId)) {
            Unfreeze();
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerServerRpc))]
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerClientRpc))]
    static bool PrefixGiantKill(int playerId) =>
        !Setting.EnableGodMode || !Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerId);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerServerRpc))]
    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerClientRpc))]
    static bool PrefixJesterKill(int playerId) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerId)) {
            Unfreeze();
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.KillPlayerAnimationServerRpc))]
    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.KillPlayerAnimationServerRpc))]
    static bool PrefixMaskedPlayerKill(int playerObjectId) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerObjectId)) {
            Unfreeze();
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerServerRpc))]
    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerClientRpc))]
    static bool PrefixDogKill(int playerId) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerId)) {
            Unfreeze();
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RedLocustBees), nameof(RedLocustBees.BeeKillPlayerServerRpc))]
    [HarmonyPatch(typeof(RedLocustBees), nameof(RedLocustBees.BeeKillPlayerClientRpc))]
    static bool PrefixBeesKill(int playerId) =>
        !Setting.EnableGodMode || !Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerId);
}
