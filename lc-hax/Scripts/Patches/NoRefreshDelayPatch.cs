using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(SteamLobbyManager), nameof(SteamLobbyManager.RefreshServerListButton))]
class NoRefreshDelayPatch {
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        bool found = false;

        foreach (CodeInstruction instruction in instructions) {
            if (instruction.opcode == OpCodes.Ldarg_0) found = true;
            if (found) yield return instruction;
        }
    }
}
