using HarmonyLib;
using UnityEngine.SceneManagement;

[HarmonyPatch(typeof(StartMatchLever), nameof(StartMatchLever.StartGame))]
sealed class LeverPatch {
    static bool Prefix(StartMatchLever __instance) {
        if (__instance.IsServer) return true;
        if (SceneManager.sceneCount > 1) return false;

        __instance.playersManager.StartGameServerRpc();
        return false;
    }
}
