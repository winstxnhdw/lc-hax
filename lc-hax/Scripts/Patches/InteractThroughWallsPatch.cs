using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SetHoverTipAndCurrentInteractTrigger))]
sealed class InteractThroughWallsPatch {
    static void Postfix(ref int ___interactableObjectsMask) =>
        ___interactableObjectsMask = LayerMask.GetMask(["Props", "InteractableObject"]);
}
