#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB))]
[HarmonyPatch("Start")]
class GrabAnywherePatch {
    static void Postfix(ref float ___interactableObjectsMask) {
        ___interactableObjectsMask = LayerMask.GetMask(["Props", "InteractableObject"]);
    }
}
