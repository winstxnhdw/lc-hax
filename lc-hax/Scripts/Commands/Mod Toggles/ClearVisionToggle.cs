using Hax;

[Command("clearvision")]
internal class ClearVisionToggle : ICommand
{
    public void Execute(StringArray _)
    {
        if (ClearVisionMod.Instance is not ClearVisionMod clearvision) return;
        clearvision.enabled = !clearvision.enabled;
        Helper.DisplayFlatHudMessage(clearvision.enabled ? "Clear Vision Mod enabled" : "Clear Vision Mod disabled");
    }
}