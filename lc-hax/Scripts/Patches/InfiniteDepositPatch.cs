using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

[HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.PlaceItemOnCounter))]
sealed class InfiniteDepositPatch {
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        foreach (CodeInstruction instruction in instructions) {
            if (instruction.LoadsConstant(150)) {
                instruction.opcode = OpCodes.Ldc_I4;
                instruction.operand = int.MaxValue;
            }

            yield return instruction;
        }
    }
}
