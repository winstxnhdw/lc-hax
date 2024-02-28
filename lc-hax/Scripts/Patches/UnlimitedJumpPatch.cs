#pragma warning disable IDE1006

using System.Collections;
using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB))]
class UnlimitedJumpPatch {
    [HarmonyPatch("Jump_performed")]
    static bool Prefix(PlayerControllerB __instance) {
        if (!Setting.EnableUnlimitedJump) return true;
        if (!__instance.IsSelf()) return true;
        if (__instance is { inSpecialInteractAnimation: true, isTypingChat: true, isPlayerControlled: false }) return true;
        if (__instance.quickMenuManager.isMenuOpen) return true;

        Reflector<PlayerControllerB> playerReflector = __instance.Reflect();

        __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter - 0.08f, 0.0f, 1.0f);
        __instance.movementAudio.PlayOneShot(StartOfRound.Instance.playerJumpSFX);

        Coroutine? jumpCoroutine = 
            playerReflector.SetInternalField("playerSlidingTimer", 0.0f)
                           .SetInternalField("isJumping", true)
                           .GetInternalField<Coroutine>("jumpCoroutine");

        if (jumpCoroutine is not null) {
            __instance.StopCoroutine(jumpCoroutine);
        }
        // create a new coroutine to handle the jump
        _ = player.SetInternalField("jumpCoroutine", __instance.StartCoroutine(playerReflector.InvokeInternalMethod<IEnumerator>("PlayerJump")));
        return false;
    }
}
