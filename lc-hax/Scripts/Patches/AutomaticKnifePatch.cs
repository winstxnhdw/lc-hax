using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;

[HarmonyPatch]
internal class AutomaticKnifePatch
{
    internal static float AttackCooldown = 0.1f;
    private static float AttackDelay = 0.0f;

    private static bool IsUsingAttack => IngamePlayerSettings.Instance.playerInput.actions
        .FindAction("ActivateItem", false).IsPressed();

    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    internal static void ItemActivatePostfix(PlayerControllerB __instance)
    {
        if (!__instance.IsSelf()) return;
        if (__instance.currentlyHeldObjectServer is GrabbableObject item && item is KnifeItem)
            if (IsUsingAttack)
            {
                AttackDelay += Time.deltaTime;
                if (AttackDelay >= AttackCooldown)
                {
                    if (item.RequireCooldown()) return;
                    AttackDelay = 0f;
                    item.UseItemOnClient(true);
                }
            }
    }

    [HarmonyPatch(typeof(KnifeItem), "HitKnife")]
    [HarmonyPrefix]
    public static void Prefix(ref KnifeItem __instance, bool cancel, ref float ___timeAtLastDamageDealt)
    {
        if (__instance.playerHeldBy is not PlayerControllerB player) return;
        if (player.IsSelf())
            if (!cancel)
                ___timeAtLastDamageDealt = 0;
    }
}