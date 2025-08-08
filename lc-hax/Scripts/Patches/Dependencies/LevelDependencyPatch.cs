using System;
using HarmonyLib;

[HarmonyPatch(typeof(RoundManager), nameof(RoundManager.FinishGeneratingNewLevelClientRpc))]
sealed class LevelDependencyPatch {
    internal static event Action? OnFinishLevelGeneration;

    static void Postfix() => LevelDependencyPatch.OnFinishLevelGeneration?.Invoke();
}
