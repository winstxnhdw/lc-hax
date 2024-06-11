#pragma warning disable IDE1006

#region

using System.Collections;
using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

[HarmonyPatch]
class PlayerActionPatches {
    [HarmonyPatch(typeof(PlayerControllerB), "Jump_performed")]
    [HarmonyPrefix]
    static bool JumpPrefix(PlayerControllerB __instance) {
        if (!Setting.EnableUnlimitedJump || !__instance.IsSelf()) return true;
        if (__instance is
            { inSpecialInteractAnimation: true, isTypingChat: true, isPlayerControlled: false }) return false;
        if (__instance.quickMenuManager.isMenuOpen) return false;
        if (__instance.isTypingChat) return false;
        __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter - 0.08f, 0.0f, 1.0f);
        __instance.movementAudio.PlayOneShot(StartOfRound.Instance.playerJumpSFX);

        Reflector<PlayerControllerB> playerReflector = __instance.Reflect();

        Coroutine? jumpCoroutine =
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
    static bool CrouchPerformedPatch(PlayerControllerB __instance, InputAction.CallbackContext context) {
        if (!__instance.IsSelf()) return true;
        Reflector<PlayerControllerB> playerReflector = __instance.Reflect();
        if (!context.performed) return false;
        if (__instance.quickMenuManager.isMenuOpen) return false;
        if (__instance.IsDeadAndNotControlled()) return false;
        if (__instance.inSpecialInteractAnimation || __instance.isTypingChat) return false;
        float crouchmeter = playerReflector.GetInternalField<float>("crouchMeter");
        playerReflector.SetInternalField("crouchMeter", Mathf.Min(crouchmeter + 0.3f, 1.3f));
        __instance.Crouch(!__instance.isCrouching);
        return false;
    }

    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Crouch))]
    [HarmonyPrefix]
    static bool OnCrouchPatch(PlayerControllerB __instance, bool crouch) {
        if (!__instance.IsSelf()) return true;
        if (__instance.IsDeadAndNotControlled()) return true;
        if(Helper.StartOfRound is not StartOfRound startOfRound) return true;
        __instance.isCrouching = crouch;
        startOfRound.timeAtMakingLastPersonalMovement = Time.realtimeSinceStartup;
        if (crouch) {
            __instance.playerBodyAnimator.SetTrigger("startCrouching");
        }
        __instance.playerBodyAnimator.SetBool("crouching", crouch);


        return false;
    }
}
