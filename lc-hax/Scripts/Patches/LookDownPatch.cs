using System.Collections.Generic;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class LookDownPatch
{
    private static IEnumerable<CodeInstruction> CalculateLookingInputTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand.Equals(60.0f)) instruction.operand = 90.0f;

            yield return instruction;
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch("CalculateSmoothLookingInput")]
    private static IEnumerable<CodeInstruction> SmoothLookingTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return CalculateLookingInputTranspiler(instructions);
    }

    [HarmonyTranspiler]
    [HarmonyPatch("CalculateNormalLookingInput")]
    private static IEnumerable<CodeInstruction> NormalLookingTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        return CalculateLookingInputTranspiler(instructions);
    }
}