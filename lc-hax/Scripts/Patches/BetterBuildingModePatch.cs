#pragma warning disable IDE1006

#region

using HarmonyLib;
using Hax;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

[HarmonyPatch]
class BetterBuildingModePatch {
    static bool ShiftKeyisPressed;
    static PlaceableShipObject? PlacingObject;

    static Keyboard Keyboard { get; } = Keyboard.current;


    [HarmonyPatch(typeof(HUDManager), "Update")]
    [HarmonyPrefix]
    static void BuldTipPrefix(HUDManager __instance) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (startOfRound.localPlayerUsingController) return;
        if (PlacingObject is not PlaceableShipObject shipobject) return;
        if (!startOfRound.unlockablesList.unlockables[shipobject.unlockableID].canBeStored)
            __instance.buildModeControlTip.text = "Reset: [Q] | Confirm: [B] | Rotate: [ R: X ¦ F: Y ¦ V: Z ]";
        else
            __instance.buildModeControlTip.text =
                "Reset: [Q] | Confirm: [B] | Rotate: [ R: X ¦ F: Y ¦ V: Z ] | Store: [X]";
    }

    [HarmonyPatch(typeof(ShipBuildModeManager), "Update")]
    [HarmonyPostfix]
    static void UpdatePostfix(ShipBuildModeManager __instance, ref PlaceableShipObject? ___placingObject) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) return;
        if (startOfRound.localPlayerUsingController) return;
        if (!__instance.InBuildMode) return;
        if (___placingObject is null) return;
        PlacingObject = ___placingObject;
        ___placingObject.AllowPlacementOnCounters = true;
        ___placingObject.AllowPlacementOnWalls = true;
        __instance.ghostObjectRenderer.sharedMaterial = __instance.ghostObjectGreen;
        if (Keyboard.leftShiftKey.isPressed)
            ShiftKeyisPressed = true;
        else
            ShiftKeyisPressed = false;
        float RotationDirection = ShiftKeyisPressed ? -155f : 155f;

        if (Keyboard.rKey.isPressed) {
            RotateObject(__instance.ghostObject, Quaternion.Euler(Time.deltaTime * RotationDirection, 0f, 0f));

            __instance.ghostObject.eulerAngles = new Vector3(
                __instance.ghostObject.eulerAngles.x,
                __instance.ghostObject.eulerAngles.y + Time.deltaTime * -155f,
                __instance.ghostObject.eulerAngles.z
            );
        }

        if (Keyboard.fKey.isPressed)
            RotateObject(__instance.ghostObject, Quaternion.Euler(0f, Time.deltaTime * RotationDirection, 0f));

        if (Keyboard.vKey.isPressed)
            RotateObject(__instance.ghostObject, Quaternion.Euler(0f, 0f, Time.deltaTime * RotationDirection));

        if (Keyboard.qKey.wasPressedThisFrame) __instance.ghostObject.eulerAngles = new Vector3(-90f, 0f, 0f);
    }

    [HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
    [HarmonyPrefix]
    static bool PlayerMeetsConditionsToBuildPrefix(ref bool __result, ref bool ___CanConfirmPosition,
        ref PlaceableShipObject? ___placingObject) {
        if (___placingObject is null) return true;

        ___placingObject.AllowPlacementOnCounters = true;
        ___placingObject.AllowPlacementOnWalls = true;
        ___CanConfirmPosition = true;
        __result = Helper.LocalPlayer is { inTerminalMenu: false };

        return false;
    }

    static void RotateObject(Transform objTransform, Quaternion rotation) => objTransform.rotation *= rotation;
}
