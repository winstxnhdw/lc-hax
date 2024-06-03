#pragma warning disable IDE1006

#region

using HarmonyLib;

#endregion

[HarmonyPatch(typeof(PatcherTool), nameof(PatcherTool.ShiftBendRandomizer))]
class ZapGunPatch {
    static void Postfix(PatcherTool __instance) => __instance.bendMultiplier = 0.0f;
}
