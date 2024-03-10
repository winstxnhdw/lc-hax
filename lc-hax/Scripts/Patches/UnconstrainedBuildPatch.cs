#pragma warning disable IDE1006

using HarmonyLib;
using Hax;
using UnityEngine;
using GameNetcodeStuff;
using UnityEngine.InputSystem;

[HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
class UnconstrainedBuildPatch {
    static bool Prefix(ref bool __result, ref bool ___CanConfirmPosition, ref PlaceableShipObject? ___placingObject) {
        if (___placingObject is null) return true;

        ___placingObject.AllowPlacementOnCounters = true;
        ___placingObject.AllowPlacementOnWalls = true;
        ___CanConfirmPosition = true;
        __result = Helper.LocalPlayer is { inTerminalMenu: false };

        return false;
    }
}

[HarmonyPatch(typeof(ShipBuildModeManager), "Update")]
class ExtraOptionsPatch {
    static Keyboard Keyboard { get; set; } = Keyboard.current;
    static bool ShiftKeyisPressed;

    static void Postfix(ShipBuildModeManager __instance) {
        if (!__instance.InBuildMode) return;
        if (ExtraOptionsPatch.Keyboard.leftShiftKey.isPressed) {
            ExtraOptionsPatch.ShiftKeyisPressed = true;
        }
        else { ExtraOptionsPatch.ShiftKeyisPressed = false; }
        float RotationDirection = ExtraOptionsPatch.ShiftKeyisPressed ? -155f : 155f;

        if (ExtraOptionsPatch.Keyboard.rKey.isPressed) {
            RotateObject(__instance.ghostObject, Quaternion.Euler(Time.deltaTime * RotationDirection, 0f, 0f));

            __instance.ghostObject.eulerAngles = new Vector3(
                __instance.ghostObject.eulerAngles.x,
                __instance.ghostObject.eulerAngles.y + Time.deltaTime * -155f,
                __instance.ghostObject.eulerAngles.z
            );
        }

        if (ExtraOptionsPatch.Keyboard.fKey.isPressed) {
            RotateObject(__instance.ghostObject, Quaternion.Euler(0f, Time.deltaTime * RotationDirection, 0f));
        }

        if (ExtraOptionsPatch.Keyboard.vKey.isPressed) {
            RotateObject(__instance.ghostObject, Quaternion.Euler(0f, 0f, Time.deltaTime * RotationDirection));
        }

        if (ExtraOptionsPatch.Keyboard.qKey.wasPressedThisFrame) {
            __instance.ghostObject.eulerAngles = new Vector3(-90f, 0f, 0f);
        }
    }
    static void RotateObject(Transform objTransform, Quaternion rotation) => objTransform.rotation *= rotation;
}

[HarmonyPatch(typeof(HUDManager), "Update")]
class HUDManagerPatch {
    static void Prefix(HUDManager __instance) => __instance.buildModeControlTip.text = "Reset: [Q] | Confirm: [B] | Rotate: [ R: X ¦ F: Y ¦ V: Z ] | Store: [X]";
}
