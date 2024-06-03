#region

using System.Collections.Generic;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;

#endregion

[HarmonyPatch(typeof(PlayerControllerB))]
class LookDownPatch {
    static IEnumerable<CodeInstruction> CalculateLookingInputTranspiler(
        IEnumerable<CodeInstruction> instructions) {
        foreach (CodeInstruction? instruction in instructions) {
            if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand.Equals(60.0f)) instruction.operand = 90.0f;

            yield return instruction;
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch("CalculateSmoothLookingInput")]
    static IEnumerable<CodeInstruction> SmoothLookingTranspiler(IEnumerable<CodeInstruction> instructions) =>
        CalculateLookingInputTranspiler(instructions);

    [HarmonyTranspiler]
    [HarmonyPatch("CalculateNormalLookingInput")]
    static IEnumerable<CodeInstruction> NormalLookingTranspiler(IEnumerable<CodeInstruction> instructions) =>
        CalculateLookingInputTranspiler(instructions);
}
