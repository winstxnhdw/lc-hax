#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB), "SetHoverTipAndCurrentInteractTrigger")]
class GrabPatch {
    static void Prefix(ref bool ___isGrabbingObjectAnimation, ref bool __state) {
        __state = ___isGrabbingObjectAnimation;
        ___isGrabbingObjectAnimation = false;
    }

    static void Postfix(
        ref bool ___isGrabbingObjectAnimation,
        ref int ___interactableObjectsMask,
        ref float ___grabDistance,
        bool __state
    ) {
        ___isGrabbingObjectAnimation = __state;
        ___interactableObjectsMask = LayerMask.GetMask(["Props", "InteractableObject"]);
        ___grabDistance = float.MaxValue;
    }
}
