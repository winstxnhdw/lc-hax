#pragma warning disable IDE1006

using System.Collections;
using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
class UnlimitedJumpPatch {
    static bool Prefix(PlayerControllerB __instance) {
        if (!Setting.EnableUnlimitedJump || !__instance.IsSelf()) return true;
        if (__instance is { inSpecialInteractAnimation: true, isTypingChat: true, isPlayerControlled: false }) return true;
        if (__instance.quickMenuManager.isMenuOpen) return true;

        __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter - 0.08f, 0.0f, 1.0f);
        __instance.movementAudio.PlayOneShot(StartOfRound.Instance.playerJumpSFX);

        Reflector<PlayerControllerB> playerReflector = __instance.Reflect();

        Coroutine? jumpCoroutine =
            playerReflector.SetInternalField("playerSlidingTimer", 0.0f)?
                           .SetInternalField("isJumping", true)?
                           .GetInternalField<Coroutine>("jumpCoroutine");

        if (jumpCoroutine is not null) {
            __instance.StopCoroutine(jumpCoroutine);
        }

        _ = playerReflector.SetInternalField(
            "jumpCoroutine",
            __instance.StartCoroutine(playerReflector.InvokeInternalMethod<IEnumerator>("PlayerJump"))
        );

        return false;
    }
}
