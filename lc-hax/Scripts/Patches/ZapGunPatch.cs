#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(PatcherTool), nameof(PatcherTool.ShiftBendRandomizer))]
class ZapGunPatch {
    static void Postfix(ref float ___bendMultiplier, ref float ___bendRandomizerShift) {
        ___bendMultiplier = 0.0f;
        ___bendRandomizerShift = 0.0f;
    }
}
