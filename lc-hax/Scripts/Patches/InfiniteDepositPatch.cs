using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

[HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.PlaceItemOnCounter))]
internal class InfiniteDepositPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
            yield return instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.Equals((sbyte)12)
                ? new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue)
                : instruction;
    }
}