#pragma warning disable IDE1006

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager))]
class ScanRangePatch {
    [HarmonyPatch("AssignNewNodes")]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        foreach (CodeInstruction instruction in instructions) {
            if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand.Equals(20.0f)) {
                instruction.operand = 500.0f;
            }

            yield return instruction;
        }
    }

    [HarmonyPatch("MeetsScanNodeRequirements")]
    static bool Prefix(ScanNodeProperties node, ref bool __result) {
        if (node == null) return true;

        __result = true;
        return false;
    }
}
