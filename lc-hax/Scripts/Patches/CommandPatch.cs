using GameNetcodeStuff;
using HarmonyLib;
using ZLinq;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.EnableChat_performed))]
sealed class EnableChatPatch {
    static void Prefix(HUDManager __instance) {
        if (__instance.localPlayer is null) return;
        __instance.chatTextField.characterLimit = int.MaxValue;
    }

    [HarmonyPostfix]
    [HarmonyInfix(typeof(PlayerControllerB), nameof(PlayerControllerB.isPlayerDead), InnerTargetKind.FieldRead)]
    static void AllowDeadPlayerChat(ref bool __result) => __result = false;
}

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.SubmitChat_performed))]
sealed class SubmitChatPatch {
    static bool Prefix() {
        if (Helper.HUDManager is not HUDManager hudManager) return true;
        if (!new[] { '!', State.CommandPrefix }.AsValueEnumerable().Any(hudManager.chatTextField.text.StartsWith)) return true;

        Chat.ExecuteCommand(hudManager.chatTextField.text.TrimEnd());
        return false;
    }

    [HarmonyPostfix]
    [HarmonyInfix(typeof(PlayerControllerB), nameof(PlayerControllerB.isPlayerDead), InnerTargetKind.FieldRead)]
    static void AllowDeadPlayerChat(ref bool __result) => __result = false;
}
