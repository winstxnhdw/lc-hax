using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.AddTextToChatOnServer))]
internal class TranslatePatch
{
    private static bool Prefix(string chatMessage, int playerId)
    {
        if (State.TranslateDetachedState is not TranslatePipe translateRequest) return true;
        if (Helper.LocalPlayer?.GetPlayerID() != playerId) return true;

        Helper.Translate(translateRequest.SourceLanguage, translateRequest.TargetLanguage, chatMessage);
        return false;
    }
}