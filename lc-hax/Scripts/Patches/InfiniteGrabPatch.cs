#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB), "SetHoverTipAndCurrentInteractTrigger")]
class InfiniteGrabPatch {
    static void Postfix(ref int ___interactableObjectsMask, ref float ___grabDistance) {
        ___interactableObjectsMask = LayerMask.GetMask(["Props", "InteractableObject"]);
        ___grabDistance = float.MaxValue;
    }
}
