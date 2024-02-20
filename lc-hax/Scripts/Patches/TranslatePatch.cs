using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.AddTextToChatOnServer))]
class TranslatePatch {
    static bool Prefix(string chatMessage, int playerId) {
        if (State.TranslateDetachedState is not TranslateDetached translateRequest) return true;
        if (Helper.LocalPlayer?.PlayerIndex() != playerId) return true;

        Helper.Translate(translateRequest.SourceLanguage, translateRequest.TargetLanguage, chatMessage);
        return false;
    }
}
