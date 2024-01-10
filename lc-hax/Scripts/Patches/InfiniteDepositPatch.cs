using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

[HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.PlaceItemOnCounter))]
class InfiniteDepositPatch {
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        foreach (CodeInstruction instruction in instructions) {
            yield return instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.Equals(12)
                ? new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue)
                : instruction;
        }
    }
}
