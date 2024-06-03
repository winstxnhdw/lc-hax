#region

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;

#endregion

[HarmonyPatch]
class LookDownSmoothPatch {
    [HarmonyPatch(typeof(PlayerControllerB), "CalculateSmoothLookingInput")]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
        instructions.Select(AdjustInstruction);

    static CodeInstruction AdjustInstruction(CodeInstruction instruction) {
        if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 60f) instruction.operand = 90f;

        return instruction;
    }
}

[HarmonyPatch]
class LookDownNormalPatch {
    [HarmonyPatch(typeof(PlayerControllerB), "CalculateNormalLookingInput")]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
        instructions.Select(AdjustInstruction);

    static CodeInstruction AdjustInstruction(CodeInstruction instruction) {
        if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 60f) instruction.operand = 90f;

        return instruction;
    }
}
