using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.TimeOutLobbyRefresh), MethodType.Enumerator)]
class TimeOutPatch {
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        foreach (CodeInstruction instruction in instructions) {
            if (instruction.opcode == OpCodes.Ldc_R4) {
                instruction.operand = 0.0f;
            }

            yield return instruction;
        }
    }
}
