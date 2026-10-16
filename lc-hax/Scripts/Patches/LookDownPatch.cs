using System.Reflection.Emit;
using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB))]
sealed class LookDownPatch {
    static IEnumerable<CodeInstruction> CalculateLookingInputTranspiler(IEnumerable<CodeInstruction> instructions, float currentLimit) {
        foreach (CodeInstruction instruction in instructions) {
            if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand.Equals(currentLimit)) {
                instruction.operand = 90.0f;
            }

            yield return instruction;
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(PlayerControllerB.CalculateSmoothLookingInput))]
    static IEnumerable<CodeInstruction> SmoothLookingTranspiler(IEnumerable<CodeInstruction> instructions) =>
        LookDownPatch.CalculateLookingInputTranspiler(instructions, 60.0f);

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(PlayerControllerB.CalculateNormalLookingInput))]
    static IEnumerable<CodeInstruction> NormalLookingTranspiler(IEnumerable<CodeInstruction> instructions) =>
        LookDownPatch.CalculateLookingInputTranspiler(instructions, 80.0f);
}
