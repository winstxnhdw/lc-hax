using Hax;

[Command("toggleweb")]
class NoSpiderWebSlownessCommand : ICommand {
    public void Execute(StringArray _) {
        Setting.DisableSpiderWebSlowness = !Setting.DisableSpiderWebSlowness;
        Helper.SendNotification("Spider Web Slowness", $"{(Setting.DisableSpiderWebSlowness ? "Disabled" : "Enabled")}");
    }
}
