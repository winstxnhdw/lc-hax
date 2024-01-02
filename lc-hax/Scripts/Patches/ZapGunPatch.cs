#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(PatcherTool), nameof(PatcherTool.ShiftBendRandomizer))]
class ZapGunPatch {
    static void Postfix(ref float ___bendMultiplier) => ___bendMultiplier = 0.0f;
}
