#pragma warning disable IDE1006

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.RequireCooldown))]
class NoCooldownPatch {
    static bool Prefix(ref bool __result) {
        __result = false;
        return false;
    }
}

[HarmonyPatch(typeof(Shovel), "reelUpShovel", MethodType.Enumerator)]
class NoShovelCooldownPatch {
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        foreach (CodeInstruction instruction in instructions) {
            if (instruction.opcode == OpCodes.Ldc_R4) {
                instruction.operand = 0.0f;
            }

            yield return instruction;
        }
    }
}
