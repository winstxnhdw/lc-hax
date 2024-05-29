using HarmonyLib;

[HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.LeaveGameConfirm))]
internal class MenuDependencyPatch
{
    private static void Postfix()
    {
        State.DisconnectedVoluntarily = true;
    }
}