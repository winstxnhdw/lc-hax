#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB))]
[HarmonyPatch("SetHoverTipAndCurrentInteractTrigger")]
class GrabPatch {
    static void Prefix(ref bool ___isGrabbingObjectAnimation) {
        ___isGrabbingObjectAnimation = true;
    }

    static void Postfix(
        ref int ___interactableObjectsMask,
        ref float ___grabDistance,
        ref bool ___isGrabbingObjectAnimation
    ) {
        ___interactableObjectsMask = LayerMask.GetMask(["Props", "InteractableObject"]);
        ___grabDistance = float.MaxValue;
        ___isGrabbingObjectAnimation = false;
    }
}
