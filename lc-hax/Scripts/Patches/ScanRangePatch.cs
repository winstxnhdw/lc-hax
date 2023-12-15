#pragma warning disable IDE1006

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager))]
class ScanRangePatch {
    [HarmonyPatch("AssignNewNodes")]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        bool foundMaxDistance = false;

        foreach (CodeInstruction instruction in instructions) {
            if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand.Equals(20.0f)) {
                instruction.operand = 50.0f;
            }

            else if (!foundMaxDistance && instruction.opcode == OpCodes.Ldc_R4 && instruction.operand.Equals(80.0f)) {
                foundMaxDistance = true;
                instruction.operand = float.MaxValue;
            }

            yield return instruction;
        }
    }

    [HarmonyPatch("MeetsScanNodeRequirements")]
    static bool Prefix(ScanNodeProperties node, ref bool __result) {
        if (node is null) return true;

        __result = true;
        return false;
    }
}
