using System.Collections.Generic;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.Update))]
sealed class PlayerInputPatch {
    static bool IsPhantomInput(PlayerControllerB player) => Setting.EnablePhantom && player.IsSelf();

    internal static bool BlocksHumanInput(PlayerControllerB player) =>
        AutoPlayMod.GetInput(player) is not null || PlayerInputPatch.IsPhantomInput(player);

    [HarmonyPostfix]
    [HarmonyInfix(typeof(InputAction), nameof(InputAction.ReadValue), Positions = [1])]
    static void ReadMove([HarmonyOuter] PlayerControllerB __instance, ref Vector2 __result) {
        if (AutoPlayMod.GetInput(__instance) is AutoPlayInput input) {
            __result = input.ReadMove();
        }

        else if (PlayerInputPatch.IsPhantomInput(__instance)) {
            __result = Vector2.zero;
        }
    }

    [HarmonyPostfix]
    [HarmonyInfix(typeof(InputAction), nameof(InputAction.ReadValue), Positions = [2])]
    static void ReadSprint([HarmonyOuter] PlayerControllerB __instance, ref float __result) {
        if (AutoPlayMod.GetInput(__instance) is AutoPlayInput input) {
            __result = input.ReadSprint();
        }

        else if (PlayerInputPatch.IsPhantomInput(__instance)) {
            __instance.inputHoldingSprint = false;
            __instance.inputLetGoOfSprint = false;
            __result = 0.0f;
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.CalculateSmoothLookingInput))]
    [HarmonyPrefix]
    static bool KeepAutoPlayDirection(PlayerControllerB __instance) => AutoPlayMod.GetInput(__instance) is null;

    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ClickHoldInteraction))]
    [HarmonyPrefix]
    static bool StopHumanHoldInteraction(PlayerControllerB __instance) {
        if (!PlayerInputPatch.BlocksHumanInput(__instance)) return true;

        __instance.isHoldingInteract = false;
        HUDManager.Instance.holdFillAmount = 0.0f;
        return false;
    }
}

[HarmonyPatch]
sealed class PlayerActionPatch {
    static IEnumerable<MethodBase> TargetMethods() => [
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.ActivateItem_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.ItemSecondaryUse_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.ItemTertiaryUse_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.Interact_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.Jump_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.Crouch_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.Discard_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.ScrollMouse_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.UseUtilitySlot_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.InspectItem_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.Emote1_performed)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.Emote2_performed))
    ];

    static bool Prefix(PlayerControllerB __instance) => !PlayerInputPatch.BlocksHumanInput(__instance);
}

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.PlayerLookInput))]
sealed class PlayerLookInputPatch {
    [HarmonyPostfix]
    [HarmonyInfix(typeof(InputAction), nameof(InputAction.ReadValue))]
    static void ReadLook([HarmonyOuter] PlayerControllerB __instance, ref Vector2 __result) {
        if (PlayerInputPatch.BlocksHumanInput(__instance)) {
            __result = Vector2.zero;
        }
    }
}
