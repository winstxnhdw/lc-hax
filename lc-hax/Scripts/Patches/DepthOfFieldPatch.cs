#pragma warning disable IDE1006

using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

[HarmonyPatch(typeof(DepthOfField), nameof(DepthOfField.IsActive))]
internal class DepthOfFieldPatch
{
    private static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}