#region

using Hax;

#endregion

[Command("toggleweb")]
class NoSpiderWebSlownessCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.DisableSpiderWebSlowness = !Setting.DisableSpiderWebSlowness;
        Helper.SendFlatNotification(Setting.DisableSpiderWebSlowness
            ? "Spider Web Slowness Disabled"
            : "Spider Web Slowness Enabled");
    }
}
