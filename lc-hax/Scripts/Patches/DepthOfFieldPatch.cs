#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(DepthOfField), nameof(DepthOfField.IsActive))]
class DepthOfFieldPatch {
    static bool Prefix(ref bool __result) {
        __result = false;
        return false;
    }
}
