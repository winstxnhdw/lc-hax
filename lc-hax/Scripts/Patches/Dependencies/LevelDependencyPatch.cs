using System;
using HarmonyLib;

[HarmonyPatch(typeof(RoundManager), nameof(RoundManager.FinishGeneratingNewLevelClientRpc))]
internal class LevelDependencyPatch
{
    internal static event Action? OnFinishLevelGeneration;

    private static void Postfix()
    {
        OnFinishLevelGeneration?.Invoke();
    }
}