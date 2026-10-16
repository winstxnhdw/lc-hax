using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SetHoverTipAndCurrentInteractTrigger))]
sealed class InfiniteGrabPatch {
    static void Postfix(PlayerControllerB __instance, ref int ___interactableObjectsMask) {
        ___interactableObjectsMask = LayerMask.GetMask(["Props", "InteractableObject"]);
        __instance.grabDistance = float.MaxValue;
    }
}
