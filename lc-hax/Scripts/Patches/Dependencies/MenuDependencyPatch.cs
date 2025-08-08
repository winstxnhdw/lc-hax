using HarmonyLib;

[HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.LeaveGameConfirm))]
sealed class MenuDependencyPatch {
    static void Postfix() => State.DisconnectedVoluntarily = true;
}
