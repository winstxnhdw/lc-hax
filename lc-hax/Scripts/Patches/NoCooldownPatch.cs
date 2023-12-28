#pragma warning disable IDE1006

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace Hax;

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
        foreach (CodeInstruction inst in instructions) {
            if (inst.opcode == OpCodes.Ldc_R4) {
                inst.operand = 0f;
            }
            yield return inst;
        }
    }
}
