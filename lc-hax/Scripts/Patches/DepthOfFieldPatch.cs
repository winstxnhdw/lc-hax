#pragma warning disable IDE1006

#region

using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

#endregion

[HarmonyPatch(typeof(DepthOfField), nameof(DepthOfField.IsActive))]
class DepthOfFieldPatch {
    static bool Prefix(ref bool __result) {
        __result = false;
        return false;
    }
}
