#region

using Hax;

#endregion

[Command("clearvision")]
class ClearVisionToggle : ICommand {
    public void Execute(StringArray _) {
        if (ClearVisionMod.Instance is not ClearVisionMod clearvision) return;
        clearvision.enabled = !clearvision.enabled;
        Helper.SendFlatNotification(clearvision.enabled ? "Clear Vision Mod enabled" : "Clear Vision Mod disabled");
    }
}
