#pragma warning disable IDE1006

using System.Collections;
using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;
using UnityEngine.InputSystem;

[HarmonyPatch]
internal class PlayerActionPatches
{
    [HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
    [HarmonyPrefix]
    private static bool JumpPrefix(PlayerControllerB __instance)
    {
        if (!Setting.EnableUnlimitedJump || !__instance.IsSelf()) return true;
        if (__instance is
            { inSpecialInteractAnimation: true, isTypingChat: true, isPlayerControlled: false }) return false;
        if (__instance.quickMenuManager.isMenuOpen) return false;
        if (__instance.isTypingChat) return false;
        __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter - 0.08f, 0.0f, 1.0f);
        __instance.movementAudio.PlayOneShot(StartOfRound.Instance.playerJumpSFX);

        var playerReflector = __instance.Reflect();

        var jumpCoroutine =
            playerReflector.SetInternalField("playerSlidingTimer", 0.0f)?
                .SetInternalField("isJumping", true)?
                .GetInternalField<Coroutine>("jumpCoroutine");

        if (jumpCoroutine is not null) __instance.StopCoroutine(jumpCoroutine);

        _ = playerReflector.SetInternalField(
            "jumpCoroutine",
            __instance.StartCoroutine(playerReflector.InvokeInternalMethod<IEnumerator>("PlayerJump"))
        );

        return false;
    }

    [HarmonyPatch(typeof(PlayerControllerB), "Crouch_performed")]
    [HarmonyPrefix]
    private static bool Prefix(PlayerControllerB __instance, InputAction.CallbackContext context)
    {
        if (!__instance.IsSelf()) return true;
        var playerReflector = __instance.Reflect();
        if (!context.performed) return false;
        if (__instance.quickMenuManager.isMenuOpen) return false;
        if ((!__instance.IsOwner || !__instance.isPlayerControlled ||
             (__instance.IsServer && !__instance.isHostPlayerObject)) && !__instance.isTestingPlayer) return false;
        if (__instance.inSpecialInteractAnimation || !__instance.thisController.isGrounded ||
            __instance.isTypingChat) return false;
        var crouchmeter = playerReflector.GetInternalField<float>("crouchMeter");
        playerReflector.SetInternalField("crouchMeter", Mathf.Min(crouchmeter + 0.3f, 1.3f));
        __instance.Crouch(!__instance.isCrouching);
        return false;
    }
}