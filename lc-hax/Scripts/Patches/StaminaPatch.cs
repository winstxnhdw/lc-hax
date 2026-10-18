using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.LateUpdate))]
sealed class StaminaPatch {
    static void Postfix(PlayerControllerB __instance) {
        if (!__instance.IsSelf()) return;

        __instance.isSpeedCheating = false;
        __instance.isExhausted = false;
        __instance.sprintMeter = 1.0f;
        __instance.sprintMeterUI.fillAmount = 1.0f;
    }
}
