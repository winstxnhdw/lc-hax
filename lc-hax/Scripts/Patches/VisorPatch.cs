using System.Collections.Generic;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.LateUpdate))]
sealed class VisorPatch {
    // Keep the visor at the game's hidden-object position instead of following the camera.
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) =>
        new CodeMatcher(instructions)
            .MatchStartForward(
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.localVisorTargetPoint))),
                new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Transform), nameof(Transform.position)))
            )
            .ThrowIfInvalid("Could not find the visor position update")
            .SetOperandAndAdvance(AccessTools.Field(typeof(PlayerControllerB), nameof(PlayerControllerB.playersManager)))
            .Insert(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StartOfRound), nameof(StartOfRound.notSpawnedPosition))))
            .InstructionEnumeration();
}
