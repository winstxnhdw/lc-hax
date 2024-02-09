using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;

[HarmonyPatch]
class GodModePatch {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath))]
    static bool Prefix(PlayerControllerB __instance, ref bool __result) {
        if (!Setting.EnableGodMode || !__instance.IsSelf()) return true;

        __result = false;
        __instance.inAnimationWithEnemy = null;
        __instance.inSpecialInteractAnimation = false;

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerServerRpc))]
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerClientRpc))]
    static bool PrefixGiantKill(int playerId) => !Setting.EnableGodMode || !Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerId);
}
