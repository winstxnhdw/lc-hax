#region

using System;
using HarmonyLib;

#endregion

[HarmonyPatch(typeof(RoundManager), nameof(RoundManager.FinishGeneratingNewLevelClientRpc))]
class LevelDependencyPatch {
    internal static event Action? OnFinishLevelGeneration;

    static void Postfix() => OnFinishLevelGeneration?.Invoke();
}
