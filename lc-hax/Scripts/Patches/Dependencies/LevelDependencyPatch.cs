using System;
using HarmonyLib;

[HarmonyPatch(typeof(RoundManager), nameof(RoundManager.FinishGeneratingNewLevelClientRpc))]
class LevelDependencyPatch {
    internal static event Action? OnFinishLevelGeneration;

    static void Postfix() => LevelDependencyPatch.OnFinishLevelGeneration?.Invoke();
}
