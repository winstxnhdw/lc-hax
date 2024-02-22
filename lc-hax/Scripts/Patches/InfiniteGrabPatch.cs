#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB), "SetHoverTipAndCurrentInteractTrigger")]
class InfiniteGrabPatch {
    static void Postfix(PlayerControllerB __instance, ref int ___interactableObjectsMask) {
        ___interactableObjectsMask = LayerMask.GetMask(["Props", "InteractableObject"]);
        __instance.grabDistance = float.MaxValue;
    }
}
