#region

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

#endregion

[HarmonyPatch(typeof(PlayerControllerB), "Update")]
class PlayerSpeedPatch {
    static void Postfix(PlayerControllerB __instance) {
        if (!__instance.IsSelf()) return;
        if (__instance.IsDead()) return;
        if (Setting.OverrideClimbSpeed) __instance.climbSpeed = Setting.New_ClimbSpeed;
        if (Setting.OverrideMovementSpeed) __instance.movementSpeed = Setting.New_MovementSpeed;
        if (Setting.OverrideJumpForce) __instance.jumpForce = Setting.New_JumpForce;
    }
}
