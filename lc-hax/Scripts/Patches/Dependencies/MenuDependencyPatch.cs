using HarmonyLib;

[HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.LeaveGameConfirm))]
class MenuDependencyPatch {
    static void Postfix() => State.DisconnectedVoluntarily = true;
}
