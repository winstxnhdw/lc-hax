namespace Hax;

static partial class Helper {
    internal static void SendNotification(string title, string body, bool isWarning = false) =>
        HUDManager?.DisplayTip(title, body, isWarning, false);

    internal static void SendFlatNotification(string msg) {
        HUDManager.globalNotificationAnimator.SetTrigger("TriggerNotif");
        HUDManager.globalNotificationText.text = msg;
        HUDManager.UIAudio.PlayOneShot(HUDManager.Instance.globalNotificationSFX);
    }
}
