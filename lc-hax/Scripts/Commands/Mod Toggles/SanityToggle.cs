using Hax;

[Command("sanity")]
internal class SanityToggle : ICommand {
    public void Execute(StringArray _) {
        if (SaneMod.Instance is not SaneMod sane) return;
        sane.enabled = !sane.enabled;
        Helper.DisplayFlatHudMessage(sane.enabled ? "Sanity Mod enabled" : "Sanity Mod disabled");
    }
}
