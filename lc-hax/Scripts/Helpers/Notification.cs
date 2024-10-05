static partial class Helper {
    [RequireNamedArgs]
    internal static void SendNotification(string title, string body, bool isWarning = false) =>
        Helper.HUDManager?.DisplayTip(title, body, isWarning, false);
}
