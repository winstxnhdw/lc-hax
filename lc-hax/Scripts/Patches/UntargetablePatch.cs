#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(EnemyAI))]
internal class UntargetableEnemyPatch
{
    [HarmonyPatch(nameof(EnemyAI.PlayerIsTargetable))]
    private static void Postfix(PlayerControllerB playerScript, ref bool __result)
    {
        if (!Setting.EnableUntargetable || !playerScript.IsSelf()) return;
        __result = false;
    }

    [HarmonyPatch(nameof(EnemyAI.Update))]
    private static bool Prefix(EnemyAI __instance)
    {
        if (!Setting.EnableUntargetable) return true;
        if (!__instance.targetPlayer.IsSelf()) return true;

        __instance.targetPlayer = null;
        __instance.movingTowardsTargetPlayer = false;
        return false;
    }
}

[HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.CheckLineOfSightForLocalPlayer))]
internal class UntargetableNutcrackerPatch
{
    private static bool Prefix()
    {
        return !Setting.EnableUntargetable;
    }
}