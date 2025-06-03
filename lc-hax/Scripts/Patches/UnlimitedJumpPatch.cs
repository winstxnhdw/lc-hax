#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
class UnlimitedJumpPatch {
    static bool Prefix(PlayerControllerB __instance) {
        if (!Setting.EnableUnlimitedJump) return true;
        if (!__instance.IsSelf()) return true;
        if (!__instance.isPlayerControlled) return false;
        if (__instance.inSpecialInteractAnimation) return false;
        if (__instance.isTypingChat) return false;
        if (__instance.quickMenuManager.isMenuOpen) return false;

        __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter - 0.08f, 0.0f, 1.0f);
        __instance.movementAudio.PlayOneShot(StartOfRound.Instance.playerJumpSFX);
        __instance.playerSlidingTimer = 0.0f;
        __instance.isJumping = true;

        Coroutine? jumpCoroutine = __instance.jumpCoroutine;

        if (jumpCoroutine is not null) {
            __instance.StopCoroutine(jumpCoroutine);
        }

        __instance.jumpCoroutine = __instance.StartCoroutine(__instance.PlayerJump());

        return false;
    }
}
