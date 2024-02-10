namespace Hax;

internal static partial class Helper {
    internal static void SendNotification(string title, string body, bool isWarning = false) =>
        Helper.HUDManager?.DisplayTip(title, body, isWarning, false);
}
