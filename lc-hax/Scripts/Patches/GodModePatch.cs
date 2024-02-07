using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;

[HarmonyPatch]
class GodModePatch {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.AllowPlayerDeath))]
    static bool PrefixDamagePlayer(PlayerControllerB __instance, ref bool __result) {
        if (!__instance.IsSelf()) return true;
        if (Setting.EnableGodMode) {
            __result = false;
            return false;
        }

        return true;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
    static bool PrefixDamagePlayer(PlayerControllerB __instance, CauseOfDeath causeOfDeath) {
        if (!__instance.IsSelf()) return true;
        if (Setting.EnableGodMode) return false;
        return !Setting.DisableFallDamage || causeOfDeath != CauseOfDeath.Gravity;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer))]
    static bool PrefixKillPlayer(PlayerControllerB __instance, CauseOfDeath causeOfDeath) {
        if (__instance.IsSelf()) return true;
        if (Setting.EnableGodMode) return false;
        return !Setting.DisableFallDamage || causeOfDeath != CauseOfDeath.Gravity;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationServerRpc))]
    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationClientRpc))]
    static bool PrefixFlowermanKill(int playerObjectId, ref bool ___startingKillAnimationLocalClient) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerObjectId)) {
            ___startingKillAnimationLocalClient = false;
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerServerRpc))]
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerClientRpc))]
    static bool PrefixGiantKill(int playerId) => !Setting.EnableGodMode || !Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerId);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerServerRpc))]
    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerClientRpc))]
    static bool PrefixJesterKill(int playerId, ref bool ___inKillAnimation) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerId)) {
            ___inKillAnimation = false;
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.KillPlayerAnimationServerRpc))]
    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.KillPlayerAnimationServerRpc))]
    static bool PrefixMaskedPlayerKill(int playerObjectId, ref bool ___startingKillAnimationLocalClient) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerObjectId)) {
            ___startingKillAnimationLocalClient = false;
            return false;
        }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerServerRpc))]
    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerClientRpc))]
    static bool PrefixDogKill(MouthDogAI __instance, int playerId, ref bool ___inKillAnimation) {
        if (!Setting.EnableGodMode) return true;
        if (Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerId)) {
            ___inKillAnimation = false;
            __instance.StopKillAnimationServerRpc();
            return false;
        }

        return true;
    }

    // needed because the dogs do break the player even on contact, which makes it impossible to grab items,
    // we patch it to make a controlled interaction for this, avoiding the death under god mode.
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.OnCollideWithPlayer))]
    static bool ControlledDogCollision(MouthDogAI __instance, ref Collider other) {
        if (!Setting.EnableGodMode) return true;
        if (__instance.IsLocalPlayerAboutToGetKilledByEnemy(other))
            if (Helper.LocalPlayer != null) {
                // instead let's make it chase the player, but never kill it.
                __instance.MouthDogChasePlayer(Helper.LocalPlayer);
                return false;
            }

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(RedLocustBees), nameof(RedLocustBees.BeeKillPlayerServerRpc))]
    [HarmonyPatch(typeof(RedLocustBees), nameof(RedLocustBees.BeeKillPlayerClientRpc))]
    static bool PrefixBeesKill(int playerId) => !Setting.EnableGodMode || !Helper.IsLocalPlayerAboutToGetKilledByEnemy(playerId);
}
