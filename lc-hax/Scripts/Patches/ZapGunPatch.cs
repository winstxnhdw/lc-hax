#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(PatcherTool), nameof(PatcherTool.ShiftBendRandomizer))]
class ZapGunPatch {
    static void Postfix(PatcherTool __instance) => __instance.bendMultiplier = 0.0f;
}
