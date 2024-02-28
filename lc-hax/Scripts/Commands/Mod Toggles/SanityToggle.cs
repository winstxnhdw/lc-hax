using Hax;

[Command("sanity")]
internal class SanityToggle : ICommand {
    public void Execute(StringArray _) {
        if (SaneMod.Instance is not SaneMod sane) return;
        sane.enabled = !sane.enabled;
        Helper.SendNotification("Sanity Mod", sane.enabled ? " enabled" : "disabled");
    }
}
