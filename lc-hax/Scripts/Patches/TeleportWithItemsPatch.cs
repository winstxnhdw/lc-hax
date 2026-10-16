using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(ShipTeleporter))]
sealed class TeleportWithItemsPatch {
    static IEnumerable<CodeInstruction> RemoveDropCall(IEnumerable<CodeInstruction> instructions, string methodName) {
        MethodInfo dropMethod = AccessTools.Method(typeof(PlayerControllerB), methodName);
        int argumentCount = dropMethod.GetParameters().Length;

        foreach (CodeInstruction instruction in instructions) {
            if (!instruction.Calls(dropMethod)) {
                yield return instruction;
                continue;
            }

            instruction.opcode = OpCodes.Pop;
            instruction.operand = null;
            yield return instruction;

            for (int i = 0; i < argumentCount; i++) {
                yield return new CodeInstruction(OpCodes.Pop);
            }
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(ShipTeleporter.TeleportPlayerOutWithInverseTeleporter))]
    static IEnumerable<CodeInstruction> InverseTeleportTranspiler(IEnumerable<CodeInstruction> instructions) =>
        TeleportWithItemsPatch.RemoveDropCall(instructions, nameof(PlayerControllerB.DropAllHeldItems));

    [HarmonyTranspiler]
    [HarmonyPatch("beamUpPlayer", MethodType.Enumerator)]
    static IEnumerable<CodeInstruction> ShipTeleportTranspiler(IEnumerable<CodeInstruction> instructions) =>
        TeleportWithItemsPatch.RemoveDropCall(instructions, nameof(PlayerControllerB.DropAllHeldItemsAndSync));
}
