using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "Update")]
internal class PlayerSpeedPatch
{
    private static void Postfix(PlayerControllerB __instance)
    {
        if (!__instance.IsSelf()) return;
        if (__instance.IsDead()) return;
        if (Setting.OverrideClimbSpeed) __instance.climbSpeed = Setting.New_ClimbSpeed;
        if (Setting.OverrideMovementSpeed) __instance.movementSpeed = Setting.New_MovementSpeed;
        if (Setting.OverrideJumpForce) __instance.jumpForce = Setting.New_JumpForce;
    }
}