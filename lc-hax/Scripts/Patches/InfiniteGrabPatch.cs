#pragma warning disable IDE1006

#region

using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

#endregion

[HarmonyPatch(typeof(PlayerControllerB), "SetHoverTipAndCurrentInteractTrigger")]
class InfiniteGrabPatch {
    static void Postfix(PlayerControllerB __instance, ref int ___interactableObjectsMask) {
        ___interactableObjectsMask = LayerMask.GetMask(["Props", "InteractableObject"]);
        __instance.grabDistance = float.MaxValue;
    }
}
