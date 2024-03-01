using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch]
internal class LookDownSmoothPatch {
    [HarmonyPatch(typeof(PlayerControllerB), "CalculateSmoothLookingInput")]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        return instructions.Select(AdjustInstruction);
    }

    private static CodeInstruction AdjustInstruction(CodeInstruction instruction) {
        if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 60f) {
            instruction.operand = 90f;
        }
        return instruction;
    }
}

[HarmonyPatch]
internal class LookDownNormalPatch {
    [HarmonyPatch(typeof(PlayerControllerB), "CalculateNormalLookingInput")]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        return instructions.Select(AdjustInstruction);
    }

    private static CodeInstruction AdjustInstruction(CodeInstruction instruction) {
        if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 60f) {
            instruction.operand = 90f;
        }
        return instruction;
    }
}
