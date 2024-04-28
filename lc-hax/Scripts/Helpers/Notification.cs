namespace Hax;

internal static partial class Helper {
    internal static void SendNotification(string title, string body, bool isWarning = false) =>
        Helper.HUDManager?.DisplayTip(title, body, isWarning, false);

    internal static void DisplayFlatHudMessage(string msg)
    {
        Helper.HUDManager.globalNotificationAnimator.SetTrigger("TriggerNotif");
        Helper.HUDManager.globalNotificationText.text = msg;
        Helper.HUDManager.UIAudio.PlayOneShot(HUDManager.Instance.globalNotificationSFX);
    }

}
