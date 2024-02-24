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
        if (!Setting.UnlimitedJump) return true;
        if(!__instance.IsSelf()) return true;
        if (__instance.quickMenuManager.isMenuOpen) return true;
        if (!__instance.isPlayerControlled) return true;
        if (__instance.inSpecialInteractAnimation) return true;
        if (__instance.isTypingChat) return true;
        Reflector player = __instance.Reflect();

        _ = player.SetInternalField("playerSlidingTimer", 0f);
        _ = player.SetInternalField("isJumping", true);
        __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter - 0.08f, 0f, 1f);
        __instance.movementAudio.PlayOneShot(StartOfRound.Instance.playerJumpSFX);
        Coroutine? jumpCoroutine = player.GetInternalField<Coroutine>("jumpCoroutine");

        if (jumpCoroutine != null) {
            __instance.StopCoroutine(jumpCoroutine);
        }
        // create a new coroutine to handle the jump
        _ = player.SetInternalField("jumpCoroutine", __instance.StartCoroutine(player.InvokeInternalMethod<IEnumerator>("PlayerJump")));
        return false;
    }
}
