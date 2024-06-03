#region

using Hax;

#endregion

[Command("sanity")]
class SanityToggle : ICommand {
    public void Execute(StringArray _) {
        if (SaneMod.Instance is not SaneMod sane) return;
        sane.enabled = !sane.enabled;
        Helper.SendFlatNotification(sane.enabled ? "Sanity Mod enabled" : "Sanity Mod disabled");
    }
}
