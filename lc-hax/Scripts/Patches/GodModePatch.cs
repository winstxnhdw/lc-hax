using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch]
public class GodModePatch {


    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath))]
    public static bool PrefixDamagePlayer(PlayerControllerB __instance, ref bool __result) {
        if (__instance.isSelf()) {
            if (Setting.EnableGodMode) {
                __result = false;
                return false;
            }
        }
        return true;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
    public static bool PrefixDamagePlayer(PlayerControllerB __instance, CauseOfDeath causeOfDeath) {
        if (__instance.isSelf()) {
            if (Setting.EnableGodMode) return false;
            if (Setting.DisableFallDamage && causeOfDeath == CauseOfDeath.Gravity) return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer))]
    public static bool PrefixKillPlayer(PlayerControllerB __instance, CauseOfDeath causeOfDeath) {
        if (__instance.isSelf()) {
            if (Setting.EnableGodMode) return false;
            if (Setting.DisableFallDamage && causeOfDeath == CauseOfDeath.Gravity) return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationServerRpc))]
    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationClientRpc))]
    public static bool PrefixFlowermanKill(int playerObjectId, ref bool ___startingKillAnimationLocalClient) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsEnemyAboutToKillLocalPlayer(playerObjectId)) {
            ___startingKillAnimationLocalClient = false;
            return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerServerRpc))]
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerClientRpc))]
    public static bool PrefixGiantKill(int playerId) {
        return !Setting.EnableGodMode || !Helper.IsEnemyAboutToKillLocalPlayer(playerId);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerServerRpc))]
    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerClientRpc))]
    public static bool PrefixJesterKill(int playerId, ref bool ___inKillAnimation) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsEnemyAboutToKillLocalPlayer(playerId)) {
            ___inKillAnimation = false;
            return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.KillPlayerAnimationServerRpc))]
    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.KillPlayerAnimationServerRpc))]
    public static bool PrefixMaskedPlayerKill(int playerObjectId, ref bool ___startingKillAnimationLocalClient) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsEnemyAboutToKillLocalPlayer(playerObjectId)) {
            ___startingKillAnimationLocalClient = false;
            return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerServerRpc))]
    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerClientRpc))]
    public static bool PrefixDogKill(int playerId, ref bool ___inKillAnimation) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsEnemyAboutToKillLocalPlayer(playerId)) {
            ___inKillAnimation = false;
            return false;
        }
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RedLocustBees), nameof(RedLocustBees.BeeKillPlayerServerRpc))]
    [HarmonyPatch(typeof(RedLocustBees), nameof(RedLocustBees.BeeKillPlayerClientRpc))]
    public static bool PrefixBeesKill(int playerId) {
        return !Setting.EnableGodMode ? true : !Helper.IsEnemyAboutToKillLocalPlayer(playerId);
    }

}
