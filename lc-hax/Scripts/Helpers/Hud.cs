namespace Hax;

internal static partial class Helper {

    internal static void DisplayNotification(string title, string Text, bool isWarning = false) => Helper.HUDManager?.DisplayTip(title, Text, isWarning, false);

}
