#region

using HarmonyLib;
using Hax;

#endregion

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.AddTextToChatOnServer))]
class TranslatePatch {
    static bool Prefix(string chatMessage, int playerId) {
        if (State.TranslateDetachedState is not TranslatePipe translateRequest) return true;
        if (Helper.LocalPlayer?.GetPlayerId() != playerId) return true;

        Helper.Translate(translateRequest.SourceLanguage, translateRequest.TargetLanguage, chatMessage);
        return false;
    }
}
