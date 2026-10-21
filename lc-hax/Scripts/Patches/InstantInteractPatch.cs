using HarmonyLib;

[HarmonyPatch(typeof(InteractTrigger), nameof(InteractTrigger.Update))]
sealed class InstantInteractPatch {
    static void Postfix(InteractTrigger __instance) {
        __instance.interactable = true;
        __instance.timeToHold = __instance.name is "EntranceTeleportB(Clone)" ? 0.3f : 0.0f;
    }
}
